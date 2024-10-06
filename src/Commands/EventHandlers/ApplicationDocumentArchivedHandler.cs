using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Domain.Application.Events;
using SmartCoinOS.Domain.Exceptions;
using SmartCoinOS.Persistence;

namespace SmartCoinOS.Commands.EventHandlers;
internal class ApplicationDocumentArchivedHandler : INotificationHandler<ApplicationDocumentArchivedEvent>
{
    private readonly DataContext _context;

    public ApplicationDocumentArchivedHandler(DataContext context)
    {
        _context = context;
    }

    public async Task Handle(ApplicationDocumentArchivedEvent notification, CancellationToken cancellationToken)
    {
        var application = await _context.Applications.FirstAsync(x => x.Id == notification.ApplicationId, cancellationToken);

        var removeDocumentResult = application.RemoveDocument(notification.DocumentId);
        if (removeDocumentResult.IsFailed)
            throw new EntityInvalidStateException("Application document couldn't be removed.");

        _context.Update(application);
    }
}
