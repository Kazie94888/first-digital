using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Domain.Application;
using SmartCoinOS.Domain.Application.Events;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Enums;
using SmartCoinOS.Domain.Exceptions;
using SmartCoinOS.Domain.SeedWork;
using SmartCoinOS.Infrastructure.Helpers.RandomStringGeneration;
using SmartCoinOS.Integrations.AzureGraphApi.Contracts;
using SmartCoinOS.Integrations.AzureGraphApi.Dto;
using SmartCoinOS.Persistence;

namespace SmartCoinOS.Commands.EventHandlers;

internal sealed class ApplicationApprovedEventHandler : INotificationHandler<ApplicationApprovedEvent>
{
    private readonly DataContext _dataContext;
    private readonly IGraphApiClient _graphApiClient;

    public ApplicationApprovedEventHandler(DataContext dataContext, IGraphApiClient graphApiClient)
    {
        _dataContext = dataContext;
        _graphApiClient = graphApiClient;
    }

    public async Task Handle(ApplicationApprovedEvent notification, CancellationToken cancellationToken)
    {
        var application = await _dataContext.Applications.FirstAsync(x => x.Id == notification.Id, cancellationToken);
        var defaultDepositBank = await _dataContext.DepositBanks.FirstOrDefaultAsync(x => x.Default, cancellationToken);

        var entityPart = application.EntityParticulars!;

        var clientBuilder = new ClientBuilder(notification.UserInfo);

        clientBuilder.AddEntityParticulars(application.LegalEntityName,
            entityPart.Particulars.RegistrationNumber,
            entityPart.Particulars.DateOfInc,
            entityPart.Particulars.LegalStructure,
            entityPart.Particulars.StructureDetails,
            entityPart.Particulars.CountryOfInc);

        clientBuilder.AddAddress(entityPart.RegistrationAddress.Country,
            entityPart.RegistrationAddress.State,
            entityPart.RegistrationAddress.PostalCode,
            entityPart.RegistrationAddress.City,
            entityPart.RegistrationAddress.Street);

        clientBuilder.AddContact(entityPart.Contact.Email, entityPart.Contact.Phone);

        clientBuilder.AddMetadata(application.ApplicationNumber.Value, defaultDepositBank?.Id);

        AppendComplianceDocuments(application, clientBuilder);

        var client = clientBuilder.Build();

        await AddUsersAsync(client, application.AuthorizedUsers, notification.UserInfo, cancellationToken);
        var bankAccounts = AddBankAccounts(client, application.BankAccounts, notification.UserInfo);
        var wallets = AddWallets(client, application.Wallets, notification.UserInfo);
        var fdtAccounts = AddFdtAccounts(client, application.FdtAccounts, notification.UserInfo);

        if (notification.UserInfo.Type is UserInfoType.BackOffice)
        {
            foreach (var bankAccount in bankAccounts)
                client.VerifyBankAccount(bankAccount.Id, notification.UserInfo);

            foreach (var wallet in wallets)
                client.VerifyWallet(wallet.Id, notification.UserInfo);

            foreach (var fdtAccount in fdtAccounts)
                client.VerifyFdtAccount(fdtAccount.Id, notification.UserInfo);

            client.VerifyEntityParticular(client.CurrentEntityParticular.Id, notification.UserInfo);
        }

        _dataContext.Clients.Add(client);
    }

    private static void AppendComplianceDocuments(Domain.Application.Application application,
        ClientBuilder clientBuilder)
    {
        if (application.Documents is null)
            return;

        var appDocs = application.Documents.OnboardingDocument;

        AddApplicationDocumentToClient(appDocs.MemorandumOfAssociation, clientBuilder);
        AddApplicationDocumentToClient(appDocs.BankStatement, clientBuilder);
        AddApplicationDocumentToClient(appDocs.CertificateOfIncumbency, clientBuilder);
        AddApplicationDocumentToClient(appDocs.CertificateOfIncAndNameChange, clientBuilder);
        AddApplicationDocumentToClient(appDocs.RegisterOfShareholder, clientBuilder);
        AddApplicationDocumentToClient(appDocs.RegisterOfDirectors, clientBuilder);
        AddApplicationDocumentToClient(appDocs.BoardResolutionAppointingAdditionalUsers, clientBuilder);

        foreach (var otherDocs in appDocs.AdditionalDocuments)
        {
            AddApplicationDocumentToClient(otherDocs, clientBuilder);
        }
    }

    private static void AddApplicationDocumentToClient(ApplicationDocument applicationDocument,
        ClientBuilder clientBuilder)
    {
        clientBuilder.AddDocument(applicationDocument.DocumentId, applicationDocument.FileName,
            applicationDocument.DocumentType);
    }

    private async Task AddUsersAsync(Client client, AuthorizedUsersForm form, UserInfo userInfo,
        CancellationToken cancellationToken)
    {
        foreach (var user in form.AuthorizedUsers)
        {
            var pwd = RandomGenerator.GenerateString();
            var azureUserId =
                await _graphApiClient.CreateUserAsync(
                    new CreateUserRequest(user.FirstName, user.LastName, user.Email, pwd), cancellationToken);

            var addUserResult =
                client.AddAuthorizedUser(user.FirstName, user.LastName, user.Email, azureUserId, userInfo);
            if (addUserResult.IsFailed)
                throw new CreateEntityException(nameof(AuthorizedUser));
        }
    }

    private List<BankAccount> AddBankAccounts(Client client, BankAccountsForm banksForm, UserInfo userInfo)
    {
        var bankAccounts = new List<BankAccount>();

        foreach (var bank in banksForm.BankRecords)
        {
            var addressRecord = new AddressRecord()
            {
                Country = bank.Country,
                State = bank.State,
                City = bank.City,
                PostalCode = bank.PostalCode,
                Street = bank.Street
            };

            var bankAccountRecord = new CreatedClientBankAccountRecord
            {
                Beneficiary = bank.Beneficiary,
                Iban = bank.Iban,
                BankName = bank.BankName,
                SwiftCode = bank.Swift,
                Alias = null,
                SmartTrustBank = new SmartTrustBank(bank.SmartTrustOwnBankId, bank.SmartTrustThirdPartyBankId),
                Address = addressRecord
            };
            
            var addBankResult = client.AddBankAccount(bankAccountRecord, userInfo);

            if (addBankResult.IsFailed)
                throw new CreateEntityException(nameof(BankAccount));

            bankAccounts.Add(addBankResult.Value);
        }

        return bankAccounts;
    }

    private List<Wallet> AddWallets(Client client, WalletsForm walletsForm, UserInfo userInfo)
    {
        var wallets = new List<Wallet>();
        foreach (var wallet in walletsForm.Wallets)
        {
            var addWalletResult = client.AddWallet(wallet.Address, wallet.Network, null, userInfo);

            if (addWalletResult.IsFailed)
                throw new CreateEntityException(nameof(Wallet));

            wallets.Add(addWalletResult.Value);
        }

        return wallets;
    }

    private List<FdtAccount> AddFdtAccounts(Client client, FdtAccountsForm fdtAccountsForm, UserInfo userInfo)
    {
        var fdtAccounts = new List<FdtAccount>();

        foreach (var fdtAccount in fdtAccountsForm.FdtAccounts)
        {
            var addFdtAccountResult =
                client.AddFdtAccount(fdtAccount.ClientName, fdtAccount.AccountNumber, null, userInfo);

            if (addFdtAccountResult.IsFailed)
                throw new CreateEntityException(nameof(FdtAccount));

            fdtAccounts.Add(addFdtAccountResult.Value);
        }

        return fdtAccounts;
    }
}
