using SmartCoinOS.Domain.Application;
using SmartCoinOS.Domain.Clients.Events;
using SmartCoinOS.Domain.Deposit;
using SmartCoinOS.Domain.SeedWork;
using SmartCoinOS.Domain.Shared;

namespace SmartCoinOS.Domain.Clients;

public sealed class Client : AggregateRoot
{
    private Client()
    {
    }

    public required ClientId Id { get; init; }
    public ClientStatus Status { get; private set; }
    public DepositBankId? DepositBankId { get; private set; }
    public required Address Address { get; init; }

    private readonly List<EntityParticular> _entityParticulars = [];
    public IReadOnlyList<EntityParticular> EntityParticulars => _entityParticulars;
    public EntityParticular CurrentEntityParticular => EntityParticulars.Single(x => !x.Archived);
    public required CompanyContact Contact { get; init; }

    private readonly List<Wallet> _wallets = [];
    public IReadOnlyList<Wallet> Wallets => _wallets;

    private readonly List<BankAccount> _bankAccounts = [];
    public IReadOnlyList<BankAccount> BankAccounts => _bankAccounts;

    private readonly List<AuthorizedUser> _authorizedUsers = [];
    public IReadOnlyList<AuthorizedUser> AuthorizedUsers => _authorizedUsers;

    private readonly List<ClientDocument> _documents = [];
    public IReadOnlyList<ClientDocument> Documents => _documents;

    private readonly List<FdtAccount> _fdtAccounts = [];
    public IReadOnlyList<FdtAccount> FdtAccounts => _fdtAccounts;

    internal static Client Create(
        EntityParticular entityParticular,
        Address registrationAddress,
        CompanyContact contact,
        string applicationNumber,
        DepositBankId? depositBankId,
        UserInfo createdBy)
    {
        var client = new Client
        {
            Id = ClientId.New(),
            Status = ClientStatus.Active,
            CreatedBy = createdBy,
            DepositBankId = depositBankId,
            Address = registrationAddress,
            Contact = contact
        };

        client._entityParticulars.Add(entityParticular);

        client.AddDomainEvent(new ClientCreatedAuditEvent(applicationNumber, entityParticular.LegalEntityName,
            client.Id, createdBy));

        return client;
    }

    public Result VerifyEntityParticular(EntityParticularId entityParticularId, UserInfo verifiedBy)
    {
        var entityParticular = _entityParticulars.Single(x => x.Id == entityParticularId && !x.Archived);

        var result = entityParticular.Verify();
        if (result.IsFailed)
            return Result.Fail(result.Errors);

        AddDomainEvent(new EntityParticularVerifiedEvent(
            entityParticularId,
            entityParticular.LegalEntityName,
            entityParticular.RegistrationNumber,
            Id,
            verifiedBy));

        return Result.Ok();
    }

    public Result DeclineEntityParticular(EntityParticularId entityParticularId, UserInfo declinedBy)
    {
        var entityParticular = _entityParticulars.Single(x => x.Id == entityParticularId && !x.Archived);

        var result = entityParticular.Decline();
        if (result.IsFailed)
            return Result.Fail(result.Errors);

        AddDomainEvent(new EntityParticularDeclinedEvent(
            entityParticularId,
            entityParticular.LegalEntityName,
            entityParticular.RegistrationNumber,
            Id,
            declinedBy));

        return Result.Ok();
    }

    public Result<EntityParticular> EditEntityParticular(string legalEntityName,
        string registrationNumber,
        DateOnly dateOfIncorporation,
        CompanyLegalStructure legalStructure,
        string countryOfInc,
        UserInfo editedBy)
    {
        if (Status != ClientStatus.Active)
            return Result.Fail(new EntityChangingStatusError(nameof(EntityParticular),
                $"Entity particular data can't be changed while in status '{Status}'"));

        var entityParticular = new EntityParticular
        {
            Id = EntityParticularId.New(),
            DateOfIncorporation = dateOfIncorporation,
            LegalEntityName = legalEntityName,
            RegistrationNumber = registrationNumber,
            LegalStructure = legalStructure,
            CountryOfInc = countryOfInc,
            CreatedBy = editedBy
        };

        _entityParticulars.ForEach(x => x.Archive());
        _entityParticulars.Add(entityParticular);

        AddDomainEvent(new EntityParticularModifiedEvent(entityParticular.Id, legalEntityName, registrationNumber, Id,
            editedBy));

        return Result.Ok(entityParticular);
    }

    public Result<AuthorizedUser> AddAuthorizedUser(string firstName, string lastName, string email, string externalId,
        UserInfo createdBy)
    {
        if (Status != ClientStatus.Active)
            return Result.Fail(new EntityChangingStatusError(nameof(AuthorizedUser),
                $"User can't be added while client is in status '{Status}'"));

        if (_authorizedUsers.Exists(x => x.Email == email))
            return Result.Fail(new EntityAlreadyExistsError(nameof(AuthorizedUsers),
                "Authorized user with given email already exists"));

        var authorizedUser = new AuthorizedUser
        {
            Id = AuthorizedUserId.New(),
            ClientId = Id,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            ExternalId = externalId,
            CreatedBy = createdBy
        };

        _authorizedUsers.Add(authorizedUser);

        AddDomainEvent(new AuthorizedUserCreatedEvent(
            firstName,
            lastName,
            email,
            Id,
            createdBy));

        return Result.Ok(authorizedUser);
    }

    public Result ArchiveAuthorizedUser(AuthorizedUserId id, UserInfo archivedBy)
    {
        if (Status != ClientStatus.Active)
            return Result.Fail(new EntityChangingStatusError(nameof(AuthorizedUser),
                $"User can't be changed while client is in status '{Status}'"));

        var user = _authorizedUsers.First(user => user.Id == id);
        var archiveResult = user.Archive();

        if (archiveResult.IsFailed)
            return archiveResult;

        AddDomainEvent(new AuthorizedUserArchivedEvent(
            id,
            user.FirstName,
            user.LastName,
            user.Email,
            Id,
            archivedBy));

        return Result.Ok();
    }

    public Result SuspendAuthorizedUser(AuthorizedUserId id, UserInfo suspendedBy)
    {
        if (Status != ClientStatus.Active)
            return Result.Fail(new EntityChangingStatusError(nameof(AuthorizedUser),
                $"User can't be suspended while client is in status '{Status}'"));

        var user = _authorizedUsers.First(user => user.Id == id);
        var archiveResult = user.Suspend();

        if (archiveResult.IsFailed)
            return archiveResult;

        AddDomainEvent(new AuthorizedUserStatusChangedEvent(user.Email, user.Status, Id, suspendedBy));

        return Result.Ok();
    }

    public Result ReactiveAuthorizedUser(AuthorizedUserId id, UserInfo reactivatedBy)
    {
        if (Status != ClientStatus.Active)
            return Result.Fail(new EntityChangingStatusError(nameof(AuthorizedUser),
                $"User can't be re-activated while client is in status '{Status}'"));

        var user = _authorizedUsers.First(user => user.Id == id);
        var archiveResult = user.Reactivate();

        if (archiveResult.IsFailed)
            return archiveResult;

        AddDomainEvent(new AuthorizedUserStatusChangedEvent(user.Email, user.Status, Id, reactivatedBy));

        return Result.Ok();
    }

    public Result<Wallet> AddWallet(string address, BlockchainNetwork network, string? alias, UserInfo createdBy)
    {
        if (Status != ClientStatus.Active)
            return Result.Fail(new EntityChangingStatusError(nameof(Wallet),
                $"Wallet can't be added while client is in status '{Status}'"));

        var walletAccount = new WalletAccount(address, network);

        if (_wallets.Exists(x => x.WalletAccount == walletAccount))
            return Result.Fail(new EntityAlreadyExistsError(nameof(Wallet), $"Wallet '{address}' already exists"));

        var wallet = new Wallet
        {
            ClientId = Id,
            Id = WalletId.New(),
            WalletAccount = walletAccount,
            Alias = alias,
            CreatedBy = createdBy
        };

        _wallets.Add(wallet);

        AddDomainEvent(new WalletCreatedEvent(wallet.Id, walletAccount, Id, createdBy));

        return Result.Ok(wallet);
    }

    public Result ArchiveWallet(WalletId walletId, bool hasActiveOrders, UserInfo archivedBy)
    {
        if (hasActiveOrders)
            return Result.Fail(new EntityInUseError(nameof(Wallet),
                "Wallet is used in active orders and can't be archived at this time."));

        if (Status != ClientStatus.Active)
            return Result.Fail(new EntityChangingStatusError(nameof(Wallet),
                $"Wallet can't be changed while client is in status '{Status}'"));

        var wallet = _wallets.First(x => x.Id == walletId);

        var archiveResult = wallet.Archive();
        if (archiveResult.IsFailed)
            return archiveResult;

        AddDomainEvent(new WalletArchivedEvent(walletId, wallet.WalletAccount, Id, archivedBy));

        return Result.Ok();
    }

    public Result VerifyWallet(WalletId walletId, UserInfo verifiedBy)
    {
        var wallet = _wallets.First(x => x.Id == walletId);

        var result = wallet.Verify();
        if (result.IsFailed)
            return Result.Fail(result.Errors);

        AddDomainEvent(new WalletVerifiedEvent(Id, verifiedBy, wallet.WalletAccount.Address));

        return Result.Ok();
    }

    public Result DeclineWallet(WalletId walletId, UserInfo declinedBy)
    {
        var wallet = _wallets.First(x => x.Id == walletId);

        var result = wallet.Decline();
        if (result.IsFailed)
            return Result.Fail(result.Errors);

        AddDomainEvent(new WalletDeclinedEvent(Id, declinedBy, wallet.WalletAccount.Address));

        return Result.Ok();
    }

    public void SetWalletOrderCount(WalletId walletId, int orderCount)
    {
        var wallet = _wallets.First(x => x.Id == walletId);
        wallet.SetOrderCount(orderCount);
    }

    public Result<BankAccount> AddBankAccount(
        CreatedClientBankAccountRecord createdClientBankAccountRecord,
        UserInfo createdBy)
    {
        if (Status != ClientStatus.Active)
            return Result.Fail(new EntityChangingStatusError(nameof(BankAccount),
                $"Bank account can't be added while client is in status '{Status}'"));

        if (createdClientBankAccountRecord.SmartTrustBank.OwnBankId is null && createdClientBankAccountRecord.SmartTrustBank.ThirdPartyBankId is null)
            return Result.Fail(new EntityInvalidError(nameof(SmartTrustBank),
                "Smart trust bank details should be provided."));
        
        var addressRecord = createdClientBankAccountRecord.Address;

        var bankAccount = new BankAccount
        {
            Id = BankAccountId.New(),
            ClientId = Id,
            Beneficiary = createdClientBankAccountRecord.Beneficiary,
            Iban = createdClientBankAccountRecord.Iban,
            BankName = createdClientBankAccountRecord.BankName,
            SwiftCode = createdClientBankAccountRecord.SwiftCode,
            CreatedBy = createdBy,
            SmartTrustBank = createdClientBankAccountRecord.SmartTrustBank,
            Address = new Address
            {
                Id = AddressId.New(),
                Country = addressRecord.Country,
                City = addressRecord.City,
                PostalCode = addressRecord.PostalCode,
                Street = addressRecord.Street,
                State = addressRecord.State,
                CreatedBy = createdBy
            }
        };
        
        if(!string.IsNullOrWhiteSpace(createdClientBankAccountRecord.Alias))
            bankAccount.SetAlias(createdClientBankAccountRecord.Alias);
        
        _bankAccounts.Add(bankAccount);
        AddDomainEvent(new BankAccountCreatedEvent(bankAccount.Id, createdClientBankAccountRecord.BankName, createdClientBankAccountRecord.Beneficiary, createdClientBankAccountRecord.Iban, createdClientBankAccountRecord.SwiftCode, Id,
            createdBy));

        return Result.Ok(bankAccount);
    }

    public Result ArchiveBankAccount(BankAccountId bankId, bool hasActiveOrders, UserInfo archivedBy)
    {
        if (hasActiveOrders)
            return Result.Fail(new EntityInUseError(nameof(BankAccount),
                "Bank account is used in active orders and can't be archived at this time."));

        if (Status != ClientStatus.Active)
            return Result.Fail(new EntityChangingStatusError(nameof(BankAccount),
                $"Bank account can't be changed while client is in status '{Status}'"));

        var bank = _bankAccounts.First(x => x.Id == bankId);

        var archiveResult = bank.Archive();
        if (archiveResult.IsFailed)
            return archiveResult;

        AddDomainEvent(new BankAccountArchivedEvent(bankId, bank.BankName, bank.Iban, Id, archivedBy));

        return Result.Ok();
    }

    public Result VerifyBankAccount(BankAccountId accountId, UserInfo verifiedBy)
    {
        var account = _bankAccounts.First(x => x.Id == accountId);

        var result = account.Verify();
        if (result.IsFailed)
            return Result.Fail(result.Errors);

        AddDomainEvent(new BankAccountVerifiedEvent(Id, verifiedBy, account.Iban));

        return Result.Ok();
    }

    public Result DeclineBankAccount(BankAccountId accountId, UserInfo declinedBy)
    {
        var account = _bankAccounts.First(x => x.Id == accountId);

        var result = account.Decline();
        if (result.IsFailed)
            return Result.Fail(result.Errors);

        AddDomainEvent(new BankAccountDeclinedEvent(Id, declinedBy, account.Iban));

        return Result.Ok();
    }

    public Result Activate(UserInfo actor)
    {
        var targetStatus = ClientStatus.Active;
        var acceptedStatusList = new[] { ClientStatus.Dormant, ClientStatus.TemporarlySuspended };

        if (!acceptedStatusList.Contains(Status))
            return Result.Fail(new EntityChangingStatusError(nameof(Client),
                $"Client status cannot be changed from {Status} to {targetStatus}"));

        if (Status == targetStatus)
            return Result.Fail(new EntityChangingStatusError(nameof(Client),
                $"Client status is already set to {targetStatus}"));

        AddDomainEvent(new ClientActivatedEvent(CurrentEntityParticular.LegalEntityName, Status, Id, actor));

        Status = targetStatus;
        return Result.Ok();
    }

    public Result Dormant(UserInfo actor)
    {
        var targetStatus = ClientStatus.Dormant;
        var acceptedStatusList = new[] { ClientStatus.Active, ClientStatus.TemporarlySuspended };

        if (!acceptedStatusList.Contains(Status))
            return Result.Fail(new EntityChangingStatusError(nameof(Client),
                $"Client status cannot be changed from {Status} to {targetStatus}"));

        if (Status == targetStatus)
            return Result.Fail(new EntityChangingStatusError(nameof(Client),
                $"Client status is already set to {targetStatus}"));

        AddDomainEvent(new ClientDormantEvent(CurrentEntityParticular.LegalEntityName, Status, Id, actor));

        Status = targetStatus;

        return Result.Ok();
    }

    public Result Suspend(bool permanent, UserInfo actor)
    {
        var targetStatus = permanent ? ClientStatus.Inactive : ClientStatus.TemporarlySuspended;

        if (Status == ClientStatus.Inactive)
            return Result.Fail(new EntityChangingStatusError(nameof(Client),
                $"Client is inactive. No further changes are permitted."));
        else if (Status == targetStatus)
            return Result.Fail(new EntityChangingStatusError(nameof(Client),
                $"Client status is already set to {targetStatus}"));

        if (targetStatus == ClientStatus.TemporarlySuspended)
            AddDomainEvent(
                new ClientTemporarlySuspendedEvent(CurrentEntityParticular.LegalEntityName, Status, Id, actor));
        else
            AddDomainEvent(new ClientDeactivatedEvent(CurrentEntityParticular.LegalEntityName, Status, Id, actor));

        Status = targetStatus;

        return Result.Ok();
    }

    public Result<ClientDocument> AddDocument(DocumentId documentId, string? fileName, string documentType,
        UserInfo createdBy)
    {
        if (Status is not (ClientStatus.Active or ClientStatus.TemporarlySuspended))
            return Result.Fail(new EntityInvalidOperation(nameof(ClientDocument),
                $"Invalid client status '{Status}' for uploading document"));

        var document = _documents.FirstOrDefault(d => d.DocumentId == documentId);
        if (document != null)
            _documents.Remove(document);
        
        document = new ClientDocument
        {
            DocumentId = documentId, DocumentType = documentType, FileName = fileName
        };
        _documents.Add(document);

        AddDomainEvent(new ClientDocumentCreatedAuditEvent(Id, document.FileName, document.DocumentId, documentType,
            createdBy));

        return Result.Ok(document);
    }

    public Result<FdtAccount> AddFdtAccount(string clientName, string fdtAccountNumber, string? alias,
        UserInfo userInfo)
    {
        if (Status != ClientStatus.Active)
            return Result.Fail(new EntityChangingStatusError(nameof(FdtAccount),
                $"FDT Account can't be added while client is in status '{Status}'"));

        var accountNumber = new FdtAccountNumber(fdtAccountNumber);
        if (_fdtAccounts.Exists(x => x.AccountNumber == accountNumber))
            return Result.Fail(new EntityAlreadyExistsError(nameof(FdtAccount),
                $"FDT Account '{fdtAccountNumber}' already exists"));

        var fdtAccount = new FdtAccount
        {
            Id = FdtAccountId.New(),
            ClientId = Id,
            ClientName = clientName,
            AccountNumber = accountNumber,
            Alias = alias,
            CreatedBy = userInfo
        };

        _fdtAccounts.Add(fdtAccount);

        AddDomainEvent(new FdtAccountCreatedEvent(accountNumber, fdtAccount.ClientName, Id, userInfo));

        return Result.Ok(fdtAccount);
    }

    public Result VerifyFdtAccount(FdtAccountId accountId, UserInfo verifiedBy)
    {
        var account = _fdtAccounts.First(x => x.Id == accountId);

        var result = account.Verify();
        if (result.IsFailed)
            return Result.Fail(result.Errors);

        AddDomainEvent(new FdtAccountVerifiedEvent(Id, verifiedBy, account.AccountNumber));

        return Result.Ok();
    }

    public Result DeclineFdtAccount(FdtAccountId accountId, UserInfo declinedBy)
    {
        var account = _fdtAccounts.First(x => x.Id == accountId);

        var result = account.Decline();
        if (result.IsFailed)
            return Result.Fail(result.Errors);

        AddDomainEvent(new FdtAccountDeclinedEvent(Id, declinedBy, account.AccountNumber));

        return Result.Ok();
    }

    public Result<FdtAccount> ArchiveFdtAccount(FdtAccountId accountId, UserInfo userInfo)
    {
        var account = _fdtAccounts.First(x => x.Id == accountId);
        var archiveResult = account.Archive();

        if (archiveResult.IsFailed)
            return Result.Fail(archiveResult.Errors);

        AddDomainEvent(new FdtAccountArchivedEvent(Id, userInfo, account.AccountNumber));

        return Result.Ok(account);
    }

    public void UpdateDepositBank(DepositBankId depositBankId, UserInfo userInfo)
    {
        DepositBankId = depositBankId;
        AddDomainEvent(new ClientDepositBankChanged(depositBankId, Id, userInfo));
    }
}
