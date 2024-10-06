using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Domain.Clients.Events;
using SmartCoinOS.Persistence;

namespace SmartCoinOS.Commands.EventHandlers;
internal sealed class ClientDocumentAddedEventHandler : INotificationHandler<ClientDocumentCreatedAuditEvent>
{
    private readonly DataContext _context;

    public ClientDocumentAddedEventHandler(DataContext context)
    {
        _context = context;
    }

    public async Task Handle(ClientDocumentCreatedAuditEvent notification, CancellationToken cancellationToken)
    {
        var document = await _context.Documents.FirstOrDefaultAsync(x => x.Id == notification.DocumentId, cancellationToken);
        if (document is null)
            return;

        document.ConnectClient(notification.ClientId, notification.DocumentType);
    }
}
