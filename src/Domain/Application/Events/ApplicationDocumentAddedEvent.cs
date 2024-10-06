using SmartCoinOS.Domain.SeedWork;


namespace SmartCoinOS.Domain.Application.Events;

public sealed class ApplicationDocumentAddedEvent : ApplicationAuditEvent
{
    public ApplicationDocumentAddedEvent(ApplicationId applicationId, List<ApplicationDocument> documentId, UserInfo auditedBy) : base(applicationId, auditedBy)
    {
        Documents = documentId;
    }

    public List<ApplicationDocument> Documents { get; }

    public override string GetDescription()
    {
        return $"{Documents.Count} documents added to application";
    }
}
