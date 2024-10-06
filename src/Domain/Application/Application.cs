using SmartCoinOS.Domain.Application.Enums;
using SmartCoinOS.Domain.Application.Events;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.SeedWork;
using SmartCoinOS.Domain.Shared;
using StronglyTypedIds;

namespace SmartCoinOS.Domain.Application;

public sealed class Application : AggregateRoot
{
    public const string AppReadOnlyMsg = "Application is in readonly state.";
    private Application() { }

    internal static Application Create(string legalEntityName, ApplicationNumber applicationNumber, UserInfo createdBy)
    {
        return new Application
        {
            Id = ApplicationId.New(),
            ApplicationNumber = applicationNumber,
            LegalEntityName = legalEntityName,
            CreatedBy = createdBy,
            Status = ApplicationStatus.Incomplete,
            Archived = false,
            BankAccounts = new(),
            Wallets = new(),
            FdtAccounts = new(),
            AuthorizedUsers = new AuthorizedUsersForm(),
        };
    }

    public required ApplicationId Id { get; init; }
    public required ApplicationNumber ApplicationNumber { get; init; }
    public required string LegalEntityName { get; init; }
    public EntityParticularsForm? EntityParticulars { get; private set; }
    public required AuthorizedUsersForm AuthorizedUsers { get; init; }
    public BusinessInfoForm? BusinessInfo { get; private set; }
    public required BankAccountsForm BankAccounts { get; init; }
    public required WalletsForm Wallets { get; init; }
    public required FdtAccountsForm FdtAccounts { get; init; }
    public DocumentsForm? Documents { get; private set; }
    public ApplicationStatus Status { get; private set; }
    public bool Archived { get; private set; }

    public Result SetDocuments(OnboardingDocumentRecord documentRecords)
    {
        if (!CanSubmitForms())
            return Result.Fail(AppReadOnlyMsg);

        Documents = new DocumentsForm
        {
            OnboardingDocument = documentRecords with { CreatedAt = DateTimeOffset.UtcNow }
        };

        return Result.Ok();
    }

    public bool CanSubmitForms()
    {
        var statusesThatAllowChange =
            new[] { ApplicationStatus.Incomplete, ApplicationStatus.AdditionalInformationRequired };
        return !Archived && statusesThatAllowChange.Contains(Status);
    }

    public Result SetEntityParticulars(CompanyLegalStructure legalStructure, string? structureDetails,
        string registrationNumber, DateOnly dateOfInc, string countryOfInc,
        List<ApplicationDocument> documents, string street, string postalCode,
        string? state, string country, string city, string email, string phone, UserInfo userInfo)
    {
        if (legalStructure == CompanyLegalStructure.Other && string.IsNullOrWhiteSpace(structureDetails))
            return Result.Fail(new EntityInvalidError(nameof(EntityParticularsForm),
                $"Structure details is required when legal structure given is other."));

        if (!CanSubmitForms())
            return Result.Fail(AppReadOnlyMsg);

        var companyParticulars = new CompanyParticulars
        {
            LegalStructure = legalStructure,
            StructureDetails = structureDetails,
            RegistrationNumber = registrationNumber,
            DateOfInc = dateOfInc,
            CountryOfInc = countryOfInc,
            Documents = documents
        };

        EntityParticulars = new EntityParticularsForm
        {
            Particulars = companyParticulars,
            RegistrationAddress = new RegistrationAddress
            {
                Street = street,
                PostalCode = postalCode,
                State = state,
                Country = country,
                City = city
            },
            Contact = new CompanyContact { Email = email, Phone = phone, }
        };

        RaiseApplicationDocumentAddedEvent(documents, userInfo);

        return Result.Ok();
    }

    public Result SetBusinessInfo(BusinessInfoForm form)
    {
        if (!CanSubmitForms())
            return Result.Fail(AppReadOnlyMsg);

        BusinessInfo = form;

        return Result.Ok();
    }

    public void ClearBankAccounts() => BankAccounts.Clear();

    public Result SetBankAccounts(string beneficiary, string bankName, string iban, string swift,
        SmartTrustBank smartTrustBank, AddressRecord address,
        List<ApplicationDocument> documents,
        UserInfo userInfo)
    {
        if (!CanSubmitForms())
            return Result.Fail(AppReadOnlyMsg);

        var record = new BankAccountRecord(beneficiary, bankName, iban, swift, smartTrustBank, address, documents);
        var addBankResult = BankAccounts.AddBankRecord(record);

        RaiseApplicationDocumentAddedEvent(documents, userInfo);

        return addBankResult;
    }

    public void ClearWallets() => Wallets.ClearWallets();

    public Result SetWallets(BlockchainNetwork network, string address)
    {
        if (!CanSubmitForms())
            return Result.Fail(AppReadOnlyMsg);

        return Wallets.AddWallet(new WalletRecord(WalletId.New(), network, address, DateTimeOffset.UtcNow));
    }

    public void ClearFdtAccounts() => FdtAccounts.ClearAccounts();

    public Result SetFdtAccounts(string clientName, string accountNumber)
    {
        if (!CanSubmitForms())
            return Result.Fail(AppReadOnlyMsg);

        return FdtAccounts.AddAccount(new FdtAccountRecord(FdtAccountId.New(), clientName, accountNumber,
            DateTimeOffset.UtcNow));
    }

    private void RaiseApplicationDocumentAddedEvent(List<ApplicationDocument> documents, UserInfo userInfo)
    {
        if (documents.Count == 0)
            return;

        AddDomainEvent(new ApplicationDocumentAddedEvent(Id, documents, userInfo));
    }

    public void ClearAuthorizedUserForm() => AuthorizedUsers.Clear();

    public Result SetAuthorizedUsersForm(string firstName, string lastName, string email,
        ApplicationDocument personalVerification, ApplicationDocument proofOfAddress, UserInfo userInfo)
    {
        if (!CanSubmitForms())
            return Result.Fail(AppReadOnlyMsg);

        var addResult = AuthorizedUsers.AddAuthorizedUser(
            firstName, lastName, email,
            personalVerification, proofOfAddress);

        if (addResult.IsFailed)
            return Result.Fail(addResult.Errors);

        RaiseApplicationDocumentAddedEvent([personalVerification, proofOfAddress], userInfo);

        return Result.Ok();
    }

    public Result Submit(UserInfo actor)
    {
        if (Status is not ApplicationStatus.Incomplete and not ApplicationStatus.AdditionalInformationRequired)
            return Result.Fail($"Application status jump from '{Status}' to 'in review' is not permited.");

        if (!AllRequiredFormsAreCompleted())
            return Result.Fail("One or more application forms are not completed.");

        Status = ApplicationStatus.InReview;
        AddDomainEvent(new ApplicationSubmittedEvent(Id, LegalEntityName, actor));

        return Result.Ok();
    }

    public Result VerifyFormRecord(ApplicationFormType form, string verifiableRecord, UserInfo userInfo)
    {
        var result = form switch
        {
            ApplicationFormType.EntityParticulars => EntityParticulars?.Verify(verifiableRecord, userInfo),
            ApplicationFormType.BusinessInfo => BusinessInfo?.Verify(verifiableRecord, userInfo),
            ApplicationFormType.AuthorizedUsers => AuthorizedUsers.Verify(verifiableRecord, userInfo),
            ApplicationFormType.BankAccounts => BankAccounts.Verify(verifiableRecord, userInfo),
            ApplicationFormType.Wallets => Wallets.Verify(verifiableRecord, userInfo),
            ApplicationFormType.FdtAccounts => FdtAccounts.Verify(verifiableRecord, userInfo),
            ApplicationFormType.Documents => Documents?.Verify(verifiableRecord, userInfo),

            _ => throw new InvalidOperationException(),
        };

        return result ?? Result.Fail("Form is not completed");
    }

    public Result Reject(UserInfo actor)
    {
        if (Status is ApplicationStatus.Approved || Status is ApplicationStatus.Rejected)
            return Result.Fail($"Approved and rejected status cannot be changed.");

        Status = ApplicationStatus.Rejected;
        Archived = true;

        AddDomainEvent(new ApplicationRejectedEvent(Id, LegalEntityName, actor));

        return Result.Ok();
    }

    public Result Approve(UserInfo actor)
    {
        if (Status is not ApplicationStatus.InReview)
            return Result.Fail($"Application at status '{Status}' cannot be approved.");

        if (!AllRequiredFormsAreCompleted())
            return Result.Fail("One or more application forms are not completed.");

        if (!AllRequiredFormsAreVerified())
            return Result.Fail("One or more application forms are not verified.");

        Status = ApplicationStatus.Approved;
        Archived = true;

        AddDomainEvent(new ApplicationApprovedEvent(Id, actor));
        AddDomainEvent(new ApplicationApprovedAuditEvent(Id, LegalEntityName, actor));

        return Result.Ok();
    }

    public Result AdditionalInfoRequired(ApplicationFormType form, UserInfo actor)
    {
        if (Status is not ApplicationStatus.InReview)
            return Result.Fail($"Only 'In Review' applications can be moved to 'Additional Info Required'.");

        if (form == ApplicationFormType.EntityParticulars)
            EntityParticulars?.RequireAdditionalInfo();
        else if (form == ApplicationFormType.AuthorizedUsers)
            AuthorizedUsers.RequireAdditionalInfo();
        else if (form == ApplicationFormType.BusinessInfo)
            BusinessInfo?.RequireAdditionalInfo();
        else if (form == ApplicationFormType.BankAccounts)
            BankAccounts.RequireAdditionalInfo();
        else if (form == ApplicationFormType.Wallets)
            Wallets.RequireAdditionalInfo();
        else if (form == ApplicationFormType.FdtAccounts)
            FdtAccounts.RequireAdditionalInfo();
        else if (form == ApplicationFormType.Documents)
            Documents?.RequireAdditionalInfo();
        else
            return Result.Fail(new EntityChangingStatusError(nameof(Application), "Unknown application form."));

        Status = ApplicationStatus.AdditionalInformationRequired;
        AddDomainEvent(new ApplicationAdditionalInfoRequiredEvent(Id, LegalEntityName, actor));

        return Result.Ok();
    }

    private bool AllRequiredFormsAreCompleted()
    {
        var foundAnEmptyForm = EntityParticulars is null
                               || BusinessInfo is null
                               || Documents is null
                               || AuthorizedUsers.AuthorizedUsers.Count == 0
                               || BankAccounts.BankRecords.Count == 0
                               || Wallets.Wallets.Count == 0;

        return !foundAnEmptyForm;
    }

    private bool AllRequiredFormsAreVerified()
    {
        var foundAnUnverifiedForm =
            EntityParticulars is null || !EntityParticulars.IsVerified()
                                      || BusinessInfo is null || !BusinessInfo.IsVerified()
                                      || Documents is null || !Documents.IsVerified()
                                      || !AuthorizedUsers.IsVerified()
                                      || !BankAccounts.IsVerified()
                                      || !Wallets.IsVerified();

        return !foundAnUnverifiedForm;
    }

    public Result RemoveDocument(DocumentId documentId)
    {
        if (!CanSubmitForms())
            return Result.Fail("Application cannot be modified at this stage.");

        EntityParticulars?.RemoveDocument(documentId);
        AuthorizedUsers.RemoveDocument(documentId);
        BusinessInfo?.RemoveDocument(documentId);
        BankAccounts.RemoveDocument(documentId);
        Documents?.RemoveDocument(documentId);

        return Result.Ok();
    }

    public Result Archive(UserInfo userInfo)
    {
        if (Archived)
            return Result.Fail("Application is already archived.");

        Archived = true;

        AddDomainEvent(new ApplicationArchivedEvent(Id, userInfo));

        return Result.Ok();
    }
}

[StronglyTypedId]
public readonly partial struct ApplicationId;
