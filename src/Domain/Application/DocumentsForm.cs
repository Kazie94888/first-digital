using System.Text.Json.Serialization;
using SmartCoinOS.Domain.Application.Enums;
using SmartCoinOS.Domain.SeedWork;
using SmartCoinOS.Domain.Shared;

namespace SmartCoinOS.Domain.Application;

public sealed class DocumentsForm : ApplicationForm
{
    internal DocumentsForm() { }

    [JsonConstructor]
    internal DocumentsForm(OnboardingDocumentRecord onboardingDocument)
    {
        OnboardingDocument = onboardingDocument;
    }

    public required OnboardingDocumentRecord OnboardingDocument { get; init; }

    internal override bool IsVerified()
    {
        return OnboardingDocument.IsVerified();
    }

    internal override void RequireAdditionalInfo()
    {
        if (!IsVerified())
            return;

        OnboardingDocument.RemoveVerificationFlag();
    }

    internal bool RemoveDocument(DocumentId documentId)
    {
        var foundDocument = OnboardingDocument.AdditionalDocuments.Find(x => x.DocumentId == documentId);
        if (foundDocument is null)
            return false;

        var removed = OnboardingDocument.AdditionalDocuments.Remove(foundDocument);
        if (removed)
            RequireAdditionalInfo();

        return removed;
    }

    internal override Result Verify(string segmentOrRecord, UserInfo userInfo)
    {
        return segmentOrRecord switch
        {
            DocumentsRecords.DueDiligenceRecords => OnboardingDocument.Verify(userInfo),

            _ => throw new InvalidOperationException()
        };
    }
}

public sealed record OnboardingDocumentRecord : VerifiableRecord
{
    public OnboardingDocumentRecord() { }

    [JsonConstructor]
    internal OnboardingDocumentRecord(ApplicationDocument memorandumOfAssociation,
        ApplicationDocument bankStatement,
        ApplicationDocument certificateOfIncumbency,
        ApplicationDocument groupOwnershipAndStructure,
        ApplicationDocument certificateOfIncAndNameChange,
        ApplicationDocument registerOfShareholder,
        ApplicationDocument registerOfDirectors,
        ApplicationDocument boardResolutionAppointingAdditionalUsers,
        List<ApplicationDocument> additionalDocuments,
        DateTimeOffset? createdAt,
        UserInfo? verifiedBy = null,
        DateTimeOffset? verifiedAt = null) : base(verifiedBy, verifiedAt)
    {
        MemorandumOfAssociation = memorandumOfAssociation;
        BankStatement = bankStatement;
        CertificateOfIncumbency = certificateOfIncumbency;
        GroupOwnershipAndStructure = groupOwnershipAndStructure;
        CertificateOfIncAndNameChange = certificateOfIncAndNameChange;
        RegisterOfShareholder = registerOfShareholder;
        RegisterOfDirectors = registerOfDirectors;
        BoardResolutionAppointingAdditionalUsers = boardResolutionAppointingAdditionalUsers;
        AdditionalDocuments = additionalDocuments;

        CreatedAt = createdAt;
    }

    public required ApplicationDocument MemorandumOfAssociation { get; init; }
    public required ApplicationDocument BankStatement { get; init; }
    public required ApplicationDocument CertificateOfIncumbency { get; init; }
    public required ApplicationDocument GroupOwnershipAndStructure { get; init; }
    public required ApplicationDocument CertificateOfIncAndNameChange { get; init; }
    public required ApplicationDocument RegisterOfShareholder { get; init; }
    public required ApplicationDocument RegisterOfDirectors { get; init; }
    public required ApplicationDocument BoardResolutionAppointingAdditionalUsers { get; init; }

    public List<ApplicationDocument> AdditionalDocuments { get; init; } = [];
    public DateTimeOffset? CreatedAt { get; init; }
}
