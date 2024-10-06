using SmartCoinOS.Domain.SeedWork;
using SmartCoinOS.Domain.Shared;

namespace SmartCoinOS.Domain.Clients.Events;

public sealed class ClientDocumentCreatedAuditEvent : ClientAuditEventBase
{
    public ClientDocumentCreatedAuditEvent(ClientId clientId, string? documentName, DocumentId documentId, string documentType, UserInfo createdBy) : base(clientId, createdBy)
    {
        ClientId = clientId;
        DocumentName = documentName;
        DocumentId = documentId;
        DocumentType = documentType;
    }

    public ClientId ClientId { get; }
    public string? DocumentName { get; set; }
    public DocumentId DocumentId { get; }
    public string DocumentType { get; set; }

    public override string GetDescription()
    {
        return $"Document {DocumentName} ({DocumentType}) uploaded";
    }
}