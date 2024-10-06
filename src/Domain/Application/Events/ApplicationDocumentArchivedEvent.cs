using MediatR;
using SmartCoinOS.Domain.Shared;

namespace SmartCoinOS.Domain.Application.Events;
public sealed class ApplicationDocumentArchivedEvent : INotification
{
    public ApplicationDocumentArchivedEvent(DocumentId documentId, ApplicationId applicationId)
    {
        DocumentId = documentId;
        ApplicationId = applicationId;
    }

    public DocumentId DocumentId { get; }
    public ApplicationId ApplicationId { get; }
}