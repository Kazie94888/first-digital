using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Domain.Application.Events;
using SmartCoinOS.Persistence;

namespace SmartCoinOS.Commands.EventHandlers;

internal sealed class ApplicationDocumentAddedEventHandler : INotificationHandler<ApplicationDocumentAddedEvent>
{
    private readonly DataContext _dataContext;

    public ApplicationDocumentAddedEventHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task Handle(ApplicationDocumentAddedEvent notification, CancellationToken cancellationToken)
    {
        var addedDocuments = notification.Documents;
        var documentIds = addedDocuments.Select(x => x.DocumentId).ToList();
        var documents = await _dataContext.Documents.Where(x => documentIds.Contains(x.Id)).ToListAsync(cancellationToken);

        if (documentIds.Count != documents.Count)
            return;

        foreach (var doc in documents)
        {
            var metadata = addedDocuments.First(x => x.DocumentId == doc.Id);
            doc.ConnectApplication(notification.ApplicationId, metadata.DocumentType);
        }
    }
}
