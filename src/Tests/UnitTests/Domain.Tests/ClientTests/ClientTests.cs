using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Clients.Events;
using SmartCoinOS.Domain.Deposit;
using SmartCoinOS.Domain.Results;
using SmartCoinOS.Domain.Shared;

namespace SmartCoinOS.Tests.Domain.ClientTests;

public sealed class ClientTests : BaseClientTest
{
    [Fact]
    public void Client_WhenCreatedFromFactory_AggregateStateIsValid()
    {
        var client = GetClientFromFactory();

        var clientHasActiveIdentity = !string.IsNullOrEmpty(client.CurrentEntityParticular.LegalEntityName) &&
                                      !string.IsNullOrEmpty(client.CurrentEntityParticular.CountryOfInc) &&
                                      !string.IsNullOrEmpty(client.CurrentEntityParticular.RegistrationNumber) &&
                                      client.CurrentEntityParticular.DateOfIncorporation != default;
        var clientHasValidAddress = !string.IsNullOrEmpty(client.Address.Country) &&
                                    !string.IsNullOrEmpty(client.Address.PostalCode) &&
                                    !string.IsNullOrEmpty(client.Address.State) &&
                                    !string.IsNullOrEmpty(client.Address.Street);

        var isInStateActive = client.Status == ClientStatus.Active;

        Assert.True(clientHasActiveIdentity);
        Assert.True(clientHasValidAddress);
        Assert.True(isInStateActive);
    }

    [Fact]
    public void Client_WhenAddingAuthorizedUser_EventIsRegistered()
    {
        var client = GetClientFromFactory();

        var firstName = "Alberto";
        var lastName = "Schuster";
        var email = "AlbertoSchuster@1stdigital.com";
        var externalId = "the-id-from-aad";

        var successResult = client.AddAuthorizedUser(firstName, lastName, email, externalId, UserInfo);
        var eventEmitted = client.DomainEvents.Any(x => x is AuthorizedUserCreatedEvent);

        Assert.True(successResult.IsSuccess);
        Assert.True(eventEmitted);
    }

    [Fact]
    public void Client_WhenAddingExistingAuthorizedUser_FailsWithError()
    {
        var client = GetClientFromFactory();

        var firstName = "Alberto";
        var lastName = "Schuster";
        var email = "AlbertoSchuster@1stdigital.com";
        var externalId = "the-id-from-aad";

        var successResult = client.AddAuthorizedUser(firstName, lastName, email, externalId, UserInfo);
        var failureResult = client.AddAuthorizedUser(firstName, lastName, email, externalId, UserInfo);

        Assert.True(successResult.IsSuccess);
        Assert.True(failureResult.IsFailed);
    }

    [Fact]
    public void Client_WhenArchiveAuthorizedUser_StatusChanged()
    {
        var client = GetClientFromFactory()
            .EnrichWithAuthorizedUser();

        var user = client.AuthorizedUsers[0];

        client.ArchiveAuthorizedUser(user.Id, UserInfo);

        Assert.Equal(AuthorizedUserStatus.Archived, user.Status);
    }

    [Fact]
    public void Client_WhenSuspendAuthorizedUser_StatusChanged()
    {
        var client = GetClientFromFactory()
            .EnrichWithAuthorizedUser();

        var user = client.AuthorizedUsers[0];

        client.SuspendAuthorizedUser(user.Id, UserInfo);

        Assert.Equal(AuthorizedUserStatus.Suspended, user.Status);
    }

    [Fact]
    public void Client_WhenReactivateAuthorizedUser_StatusChanged()
    {
        var client = GetClientFromFactory()
            .EnrichWithAuthorizedUser();

        var user = client.AuthorizedUsers[0];

        client.SuspendAuthorizedUser(user.Id, UserInfo);

        Assert.Equal(AuthorizedUserStatus.Suspended, user.Status);

        client.ReactiveAuthorizedUser(user.Id, UserInfo);

        Assert.Equal(AuthorizedUserStatus.Active, user.Status);
    }

    [Fact]
    public void Client_WhenArchiveAuthorizedUser_EventIsRegistered()
    {
        var client = GetClientFromFactory()
            .EnrichWithAuthorizedUser();

        var clientId = client.AuthorizedUsers[0].Id;

        client.ArchiveAuthorizedUser(clientId, UserInfo);

        var eventEmitted = client.DomainEvents.Any(x => x is AuthorizedUserArchivedEvent);

        Assert.True(eventEmitted);
    }

    [Fact]
    public void Client_WhenSuspendingArchivedUser_Fails()
    {
        var client = GetClientFromFactory()
            .EnrichWithAuthorizedUser();

        var clientId = client.AuthorizedUsers[0].Id;

        client.ArchiveAuthorizedUser(clientId, UserInfo);

        var result = client.SuspendAuthorizedUser(clientId, UserInfo);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public void Client_WhenReactivatingArchivedUser_Fails()
    {
        var client = GetClientFromFactory()
            .EnrichWithAuthorizedUser();

        var clientId = client.AuthorizedUsers[0].Id;

        client.ArchiveAuthorizedUser(clientId, UserInfo);

        var result = client.ReactiveAuthorizedUser(clientId, UserInfo);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public void Client_WhenArchivingBankAccountInUse_Fails()
    {
        var client = GetClientFromFactory();

        var result = client.ArchiveBankAccount(BankAccountId.New(), true, UserInfo);
        Assert.True(result.IsFailed);
        Assert.True(result.HasError<EntityInUseError>());
    }

    [Fact]
    public void Client_WhenArchivingBankAccount_AuditIsRegistered()
    {
        var client = GetClientFromFactory()
                        .EnrichWithBankAccount();

        var activeBank = client.BankAccounts.First(x => !x.Archived);
        var archiveResult = client.ArchiveBankAccount(activeBank.Id, false, UserInfo);
        var auditEventIsEmitted = client.DomainEvents.Any(e => e is BankAccountArchivedEvent);

        Assert.True(archiveResult.IsSuccess);
        Assert.True(auditEventIsEmitted);
    }

    [Fact]
    public void Client_WhenArchivingBankAccountArchived_FailsWithError()
    {
        var client = GetClientFromFactory()
                        .EnrichWithBankAccount();

        var bankId = client.BankAccounts.Select(x => x.Id).First();
        var successResult = client.ArchiveBankAccount(bankId, false, UserInfo);
        var failureResult = client.ArchiveBankAccount(bankId, false, UserInfo);

        Assert.True(successResult.IsSuccess);
        Assert.True(failureResult.IsFailed);
    }

    [Fact]
    public void Client_WhenAddingBankAccount_BankExistsNotArchived()
    {
        var client = GetClientFromFactory()
                        .EnrichWithBankAccount();

        var clientBank = client.BankAccounts[0];

        Assert.NotNull(clientBank);
        Assert.False(clientBank.Archived);
    }

    [Fact]
    public void Client_WhenArchivingWalletInUse_Fails()
    {
        var client = GetClientFromFactory();

        var result = client.ArchiveWallet(WalletId.New(), true, UserInfo);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public void Client_WhenArchivingWallet_AuditIsRegistered()
    {
        var client = GetClientFromFactory().EnrichWithWallet();

        var activeWallet = client.Wallets.First(x => !x.Archived);
        var archiveResult = client.ArchiveWallet(activeWallet.Id, false, UserInfo);
        var auditEventIsEmitted = client.DomainEvents.Any(x => x is WalletArchivedEvent);

        Assert.True(archiveResult.IsSuccess);
        Assert.True(auditEventIsEmitted);
    }

    [Fact]
    public void Client_WhenArchivingWalletArchived_FailsWithError()
    {
        var client = GetClientFromFactory().EnrichWithWallet();

        var activeWallet = client.Wallets.First(x => !x.Archived);
        var successResult = client.ArchiveWallet(activeWallet.Id, false, UserInfo);
        var failureResult = client.ArchiveWallet(activeWallet.Id, false, UserInfo);

        Assert.True(successResult.IsSuccess);
        Assert.True(failureResult.IsFailed);
    }

    [Fact]
    public void Client_WhenNewWalletExists_ReturnsError()
    {
        var client = GetClientFromFactory().EnrichWithWallet();

        var existingWallet = client.Wallets[0];
        var addResult = client.AddWallet(existingWallet.WalletAccount.Address, existingWallet.WalletAccount.Network, null, UserInfo);
        var hasExistingEntityError = addResult.HasError(x => x is EntityAlreadyExistsError);

        Assert.True(addResult.IsFailed);
        Assert.True(hasExistingEntityError);
    }

    [Fact]
    public void Client_WhenWalletAdded_EventEmitted()
    {
        var client = GetClientFromFactory().EnrichWithWallet();

        var networkId = "0xa444f5ea0ba59494ce839613ffhba74279589269";
        var addResult = client.AddWallet(networkId, BlockchainNetwork.SepoliaTestnet, "Identifier Name", UserInfo);
        var eventExists = client.DomainEvents.Any(x => x is WalletCreatedEvent);

        Assert.True(addResult.IsSuccess);
        Assert.True(eventExists);
    }

    [Fact]
    public void Client_WhenBankAccountCreated_EventIsEmitted()
    {
        var client = GetClientFromFactory().EnrichWithBankAccount();

        var hasEvent = client.DomainEvents.Any(e => e is BankAccountCreatedEvent);

        Assert.True(hasEvent);
    }

    [Fact]
    public void Client_WhenFdtAccountIsCreated_EventIsEmitted()
    {
        var client = GetClientFromFactory().EnrichWithFdtAccount();

        var hasEvent = client.DomainEvents.Any(x => x is FdtAccountCreatedEvent);

        Assert.True(hasEvent);
    }

    [Fact]
    public void Client_WhenFdtAccountIsAlreadyArchived_FailsWithError()
    {
        var client = GetClientFromFactory().EnrichWithFdtAccount();

        var successfulArchive = client.ArchiveFdtAccount(client.FdtAccounts[0].Id, UserInfo);
        var failedArchive = client.ArchiveFdtAccount(client.FdtAccounts[0].Id, UserInfo);

        Assert.True(successfulArchive.IsSuccess);
        Assert.True(failedArchive.IsFailed);
    }

    [Fact]
    public void Client_WhenFdtAccountIsArchived_EventIsEmitted()
    {
        var client = GetClientFromFactory().EnrichWithFdtAccount();

        var archiveAction = client.ArchiveFdtAccount(client.FdtAccounts[0].Id, UserInfo);
        var hasArchivedEvent = client.DomainEvents.Any(x => x is FdtAccountArchivedEvent);

        Assert.True(archiveAction.IsSuccess);
        Assert.True(hasArchivedEvent);
    }

    [Fact]
    public void UpdateDepositBank_WhenSuccess_EventIsEmitted()
    {
        var client = GetClientFromFactory();

        client.UpdateDepositBank(DepositBankId.New(), UserInfo);
        var eventFound = client.DomainEvents.Any(x => x is ClientDepositBankChanged);

        Assert.True(eventFound);
    }

    [Fact]
    public void VerifyEntityParticular_WhenSuccess_EventIsEmitted()
    {
        var client = GetClientFromFactory();

        var verificationAction = client.VerifyEntityParticular(client.CurrentEntityParticular.Id, UserInfo);
        var hasVerificationEvent = client.DomainEvents.Any(x => x is EntityParticularVerifiedEvent);

        Assert.True(verificationAction.IsSuccess);
        Assert.True(hasVerificationEvent);
    }

    [Fact]
    public void VerifyWallet_WhenSuccess_EventIsEmitted()
    {
        var client = GetClientFromFactory().EnrichWithWallet();

        var verificationAction = client.VerifyWallet(client.Wallets[0].Id, UserInfo);
        var hasVerificationEvent = client.DomainEvents.Any(x => x is WalletVerifiedEvent);

        Assert.True(verificationAction.IsSuccess);
        Assert.True(hasVerificationEvent);
    }

    [Fact]
    public void VerifyBankAccount_WhenSuccess_EventIsEmitted()
    {
        var client = GetClientFromFactory().EnrichWithBankAccount();

        var verificationAction = client.VerifyBankAccount(client.BankAccounts[0].Id, UserInfo);
        var hasVerificationEvent = client.DomainEvents.Any(x => x is BankAccountVerifiedEvent);

        Assert.True(verificationAction.IsSuccess);
        Assert.True(hasVerificationEvent);
    }

    [Fact]
    public void VerifyFdtAccount_WhenSuccess_EventIsEmitted()
    {
        var client = GetClientFromFactory().EnrichWithFdtAccount();

        var verificationAction = client.VerifyFdtAccount(client.FdtAccounts[0].Id, UserInfo);
        var hasVerificationEvent = client.DomainEvents.Any(x => x is FdtAccountVerifiedEvent);

        Assert.True(verificationAction.IsSuccess);
        Assert.True(hasVerificationEvent);
    }

    [Fact]
    public void VerifyEntityParticular_WhenClientAssetCreatedByAdminUser_FailsWithError()
    {
        var client = GetClientFromFactory(verifyEntityParticular: true);

        var verificationAction = client.VerifyEntityParticular(client.CurrentEntityParticular.Id, UserInfo);

        Assert.True(verificationAction.IsFailed);
    }

    [Fact]
    public void VerifyWallet_WhenClientAssetCreatedByAdminUser_FailsWithError()
    {
        var client = GetClientFromFactory().EnrichWithWallet(true);

        var verificationAction = client.VerifyWallet(client.Wallets[0].Id, UserInfo);

        Assert.True(verificationAction.IsFailed);
    }

    [Fact]
    public void VerifyBankAccount_WhenClientAssetCreatedByAdminUser_FailsWithError()
    {
        var client = GetClientFromFactory().EnrichWithBankAccount(true);

        var verificationAction = client.VerifyBankAccount(client.BankAccounts[0].Id, UserInfo);

        Assert.True(verificationAction.IsFailed);
    }

    [Fact]
    public void VerifyFdtAccount_WhenClientAssetCreatedByAdminUser_FailsWithError()
    {
        var client = GetClientFromFactory().EnrichWithFdtAccount(true);

        var verificationAction = client.VerifyFdtAccount(client.FdtAccounts[0].Id, UserInfo);

        Assert.True(verificationAction.IsFailed);
    }


    [Fact]
    public void Client_WhenWalletIsArchived_VerificationFails()
    {
        var client = GetClientFromFactory().EnrichWithWallet();

        var successfulArchive = client.ArchiveWallet(client.Wallets[0].Id, false, UserInfo);
        var failedVerification = client.VerifyWallet(client.Wallets[0].Id, UserInfo);

        Assert.True(successfulArchive.IsSuccess);
        Assert.True(failedVerification.IsFailed);
    }


    [Fact]
    public void Client_WhenBankAccountIsArchived_VerificationFails()
    {
        var client = GetClientFromFactory().EnrichWithBankAccount();

        var successfulArchive = client.ArchiveBankAccount(client.BankAccounts[0].Id, false, UserInfo);
        var failedVerification = client.VerifyBankAccount(client.BankAccounts[0].Id, UserInfo);

        Assert.True(successfulArchive.IsSuccess);
        Assert.True(failedVerification.IsFailed);
    }


    [Fact]
    public void Client_WhenFdtAccountIsArchived_VerificationFails()
    {
        var client = GetClientFromFactory().EnrichWithFdtAccount();

        var successfulArchive = client.ArchiveFdtAccount(client.FdtAccounts[0].Id, UserInfo);
        var failedVerification = client.VerifyFdtAccount(client.FdtAccounts[0].Id, UserInfo);

        Assert.True(successfulArchive.IsSuccess);
        Assert.True(failedVerification.IsFailed);
    }

    [Fact]
    public void DeclineEntityParticular_WhenSuccess_EventIsEmitted()
    {
        var client = GetClientFromFactory();

        var declinationResult = client.DeclineEntityParticular(client.CurrentEntityParticular.Id, UserInfo);
        var hasDeclinationEvent = client.DomainEvents.Any(x => x is EntityParticularDeclinedEvent);

        Assert.True(declinationResult.IsSuccess);
        Assert.True(hasDeclinationEvent);
    }

    [Fact]
    public void DeclineWallet_WhenSuccess_EventIsEmitted()
    {
        var client = GetClientFromFactory().EnrichWithWallet();

        var declinationResult = client.DeclineWallet(client.Wallets[0].Id, UserInfo);
        var hasDeclinationEvent = client.DomainEvents.Any(x => x is WalletDeclinedEvent);

        Assert.True(declinationResult.IsSuccess);
        Assert.True(hasDeclinationEvent);
    }

    [Fact]
    public void DeclineBankAccount_WhenSuccess_EventIsEmitted()
    {
        var client = GetClientFromFactory().EnrichWithBankAccount();

        var declinationResult = client.DeclineBankAccount(client.BankAccounts[0].Id, UserInfo);
        var hasDeclinationEvent = client.DomainEvents.Any(x => x is BankAccountDeclinedEvent);

        Assert.True(declinationResult.IsSuccess);
        Assert.True(hasDeclinationEvent);
    }

    [Fact]
    public void DeclineFdtAccount_WhenSuccess_EventIsEmitted()
    {
        var client = GetClientFromFactory().EnrichWithFdtAccount();

        var declinationResult = client.DeclineFdtAccount(client.FdtAccounts[0].Id, UserInfo);
        var hasDeclinationEvent = client.DomainEvents.Any(x => x is FdtAccountDeclinedEvent);

        Assert.True(declinationResult.IsSuccess);
        Assert.True(hasDeclinationEvent);
    }

    [Fact]
    public void Client_WhenWalletIsArchived_DeclinationFails()
    {
        var client = GetClientFromFactory().EnrichWithWallet();

        var successfulArchive = client.ArchiveWallet(client.Wallets[0].Id, false, UserInfo);
        var failedDeclination = client.DeclineWallet(client.Wallets[0].Id, UserInfo);

        Assert.True(successfulArchive.IsSuccess);
        Assert.True(failedDeclination.IsFailed);
    }


    [Fact]
    public void Client_WhenBankAccountIsArchived_DeclinationFails()
    {
        var client = GetClientFromFactory().EnrichWithBankAccount();

        var successfulArchive = client.ArchiveBankAccount(client.BankAccounts[0].Id, false, UserInfo);
        var failedDeclination = client.DeclineBankAccount(client.BankAccounts[0].Id, UserInfo);

        Assert.True(successfulArchive.IsSuccess);
        Assert.True(failedDeclination.IsFailed);
    }

    [Fact]
    public void Client_WhenFdtAccountIsArchived_DeclinationFails()
    {
        var client = GetClientFromFactory().EnrichWithFdtAccount();

        var successfulArchive = client.ArchiveFdtAccount(client.FdtAccounts[0].Id, UserInfo);
        var failedDeclination = client.DeclineFdtAccount(client.FdtAccounts[0].Id, UserInfo);

        Assert.True(successfulArchive.IsSuccess);
        Assert.True(failedDeclination.IsFailed);
    }

    [Theory]
    [InlineData(EntityVerificationStatus.Verified)]
    [InlineData(EntityVerificationStatus.Declined)]
    public void ChangeEntityParticularStatus_WhenEntityParticularIsAlreadyDeclined_FailsWithError(EntityVerificationStatus verificationStatus)
    {
        var client = GetClientFromFactory();

        var successfulDeclination = client.DeclineEntityParticular(client.CurrentEntityParticular.Id, UserInfo);

        var failedAction = verificationStatus switch
        {
            EntityVerificationStatus.Verified => client.VerifyEntityParticular(client.CurrentEntityParticular.Id, UserInfo),
            EntityVerificationStatus.Declined => client.DeclineEntityParticular(client.CurrentEntityParticular.Id, UserInfo),
            _ => throw new ArgumentException("Invalid method argument", nameof(verificationStatus))
        };

        Assert.True(successfulDeclination.IsSuccess);
        Assert.True(failedAction.IsFailed);
    }

    [Theory]
    [InlineData(EntityVerificationStatus.Verified)]
    [InlineData(EntityVerificationStatus.Declined)]
    public void ChangeWalletStatus_WhenWalletIsAlreadyDeclined_FailsWithError(EntityVerificationStatus verificationStatus)
    {
        var client = GetClientFromFactory().EnrichWithWallet();

        var successfulDeclination = client.DeclineWallet(client.Wallets[0].Id, UserInfo);

        var failedAction = verificationStatus switch
        {
            EntityVerificationStatus.Verified => client.VerifyWallet(client.Wallets[0].Id, UserInfo),
            EntityVerificationStatus.Declined => client.DeclineWallet(client.Wallets[0].Id, UserInfo),
            _ => throw new ArgumentException("Invalid method argument", nameof(verificationStatus))
        };

        Assert.True(successfulDeclination.IsSuccess);
        Assert.True(failedAction.IsFailed);
    }

    [Theory]
    [InlineData(EntityVerificationStatus.Verified)]
    [InlineData(EntityVerificationStatus.Declined)]
    public void ChangeBankAccountStatus_WhenBankAccountIsAlreadyDeclined_FailsWithError(EntityVerificationStatus verificationStatus)
    {
        var client = GetClientFromFactory().EnrichWithBankAccount();

        var successfulDeclination = client.DeclineBankAccount(client.BankAccounts[0].Id, UserInfo);

        var failedAction = verificationStatus switch
        {
            EntityVerificationStatus.Verified => client.VerifyBankAccount(client.BankAccounts[0].Id, UserInfo),
            EntityVerificationStatus.Declined => client.DeclineBankAccount(client.BankAccounts[0].Id, UserInfo),
            _ => throw new ArgumentException("Invalid method argument", nameof(verificationStatus))
        };

        Assert.True(successfulDeclination.IsSuccess);
        Assert.True(failedAction.IsFailed);
    }

    [Theory]
    [InlineData(EntityVerificationStatus.Verified)]
    [InlineData(EntityVerificationStatus.Declined)]
    public void ChangeFdtAccountStatus_WhenFdtAccountIsAlreadyDeclined_FailsWithError(EntityVerificationStatus verificationStatus)
    {
        var client = GetClientFromFactory().EnrichWithFdtAccount();

        var successfulDeclination = client.DeclineFdtAccount(client.FdtAccounts[0].Id, UserInfo);

        var failedAction = verificationStatus switch
        {
            EntityVerificationStatus.Verified => client.VerifyFdtAccount(client.FdtAccounts[0].Id, UserInfo),
            EntityVerificationStatus.Declined => client.DeclineFdtAccount(client.FdtAccounts[0].Id, UserInfo),
            _ => throw new ArgumentException("Invalid method argument", nameof(verificationStatus))
        };

        Assert.True(successfulDeclination.IsSuccess);
        Assert.True(failedAction.IsFailed);
    }

    [Fact]
    public void AddDocument_WithValidData_Successfully()
    {
        var client = GetClientFromFactory();
        var documentId = DocumentId.New();
        var result = client.AddDocument(documentId, "test_upload_image.png", "image/png", UserInfo);

        Assert.Equal(documentId, result.Value.DocumentId);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void AddDocument_WhenSuccess_EventIsEmitted()
    {
        var client = GetClientFromFactory();

        var result = client.AddDocument(DocumentId.New(), "test_upload_image.png", "image/png", UserInfo);

        Assert.True(result.IsSuccess);
        Assert.Contains(client.DomainEvents, x => x is ClientDocumentCreatedAuditEvent);
    }

    [Fact]
    public void AddDocument_WhenStatusIsNotValidForUploading_ReturnsFailure()
    {
        var client = GetClientFromFactory();
        client.Suspend(true, UserInfo);

        var result = client.AddDocument(DocumentId.New(), "test_upload_image.png", "image/png", UserInfo);

        Assert.True(result.IsFailed);
        Assert.True(result.HasError(x => x is EntityInvalidOperation));
    }
    
    [Fact]
    public void AddDocument_WhenStatusIsTemporarilySuspended_Successfully()
    {
        var client = GetClientFromFactory();
        client.Suspend(false, UserInfo);

        var documentId = DocumentId.New();
        var result = client.AddDocument(documentId, "test_upload_image.png", "image/png", UserInfo);

        Assert.True(result.IsSuccess);
        Assert.Equal(documentId, result.Value.DocumentId);
        Assert.Contains(client.DomainEvents, x => x is ClientDocumentCreatedAuditEvent);
    }
    
    [Fact]
    public void CreateDocument_WhenFileContentIsInvalid_ReturnsError()
    {
        var result = Document.Validate("application/vnd.microsoft.portable-executable",1487 );

        Assert.True(result.IsFailed);
        Assert.True(result.HasError(x => x is InvalidContentTypeError));
    }

    [Fact]
    public void CreateDocument_WhenFileSizeOverRequired_ReturnsError()
    {
        var result = Document.Validate("image/png", 10485761);

        Assert.True(result.IsFailed);
        Assert.True(result.HasError(x => x is InvalidSizeInBytesError));
    }
}