using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Compliance;

internal sealed class UploadComplianceDocumentsCommandHandler : ICommandHandler<UploadComplianceDocumentsCommand, List<DocumentDto>>
{
    private readonly DataContext _context;

    public UploadComplianceDocumentsCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<List<DocumentDto>>> Handle(UploadComplianceDocumentsCommand command, CancellationToken cancellationToken)
    {
        var client = await _context.Clients.FirstAsync(x => x.Id == command.ClientId, cancellationToken);

        foreach (var document in command.Documents)
        {
            var addDocumentResult = client.AddDocument(document.FileId, document.FileName, document.DocumentType, command.UserInfo);

            if (addDocumentResult.IsFailed)
                return Result.Fail(addDocumentResult.Errors);
        }

        _context.Update(client);

        return Result.Ok(command.Documents);
    }
}