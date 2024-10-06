using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.ClientPortal.Api.Base.Dto;
using SmartCoinOS.Domain.Application;
using SmartCoinOS.Domain.Results;
using SmartCoinOS.Domain.Shared;
using SmartCoinOS.Persistence;

namespace SmartCoinOS.ClientPortal.Api.Features.FileStorage;

internal sealed class DeleteDocumentCommandHandler : ICommandHandler<DeleteDocumentCommand, EntityId>
{
    private readonly DataContext _context;

    public DeleteDocumentCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<EntityId>> Handle(DeleteDocumentCommand command, CancellationToken cancellationToken)
    {
        var documentId = command.Request.Id;
        var document = await _context.Documents.FirstAsync(
                x => x.Id == documentId && x.ClientId == command.UserInfo.ClientId,
            cancellationToken);

        var canDelete = await CanDeleteDocumentAsync(document, cancellationToken);

        if (!canDelete)
            return Result.Fail(new EntityInUseError(nameof(ApplicationDocument),
                "Application Document is in use and can't be deleted."));

        var archiveResult = document.Archive();
        if (archiveResult.IsFailed)
            return archiveResult;

        var archivedDocument = new EntityId(document.Id.Value);
        return Result.Ok(archivedDocument);
    }

    private async Task<bool> CanDeleteDocumentAsync(Document document, CancellationToken cancellationToken)
    {
        if (!document.ApplicationId.HasValue)
            return true;

        var application = await _context.Applications.FirstAsync(x => x.Id == document.ApplicationId, cancellationToken);
        
        return application.CanSubmitForms();
    }
}
