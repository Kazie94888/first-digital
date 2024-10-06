using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Application;
using SmartCoinOS.Domain.Results;
using SmartCoinOS.Domain.Shared;

namespace SmartCoinOS.Backoffice.Api.Features.FileStorage;

internal sealed class DeleteFileCommandHandler : ICommandHandler<DeleteFileCommand, EntityId>
{
    private readonly DataContext _context;

    public DeleteFileCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<EntityId>> Handle(DeleteFileCommand command, CancellationToken cancellationToken)
    {
        var documentId = command.Request.Id;
        var document = await _context.Documents.FirstAsync(x => x.Id == documentId, cancellationToken);

        var applicationDocAllowsDelete = await ApplicationDocumentCanBeDeletedAsync(document, cancellationToken);

        if (!applicationDocAllowsDelete)
            return Result.Fail(new EntityInUseError(nameof(ApplicationDocument), "Application Document is in use and can't be deleted."));

        var archiveResult = document.Archive();
        if (archiveResult.IsFailed)
            return archiveResult;

        var archivedDocument = new EntityId(document.Id.Value);
        return Result.Ok(archivedDocument);
    }

    private async Task<bool> ApplicationDocumentCanBeDeletedAsync(Document document, CancellationToken cancellationToken)
    {
        if (!document.ApplicationId.HasValue)
            return true;

        var application = await _context.Applications.FirstAsync(x => x.Id == document.ApplicationId, cancellationToken);
        return application.CanSubmitForms();
    }
}
