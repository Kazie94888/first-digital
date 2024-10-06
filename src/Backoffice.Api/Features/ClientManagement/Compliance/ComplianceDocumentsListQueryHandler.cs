using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Base;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Compliance;

internal sealed class ComplianceDocumentsListQueryHandler : IQueryHandler<ComplianceDocumentsListQuery, InfoList<ComplianceDocumentsItem>>
{
    private readonly ReadOnlyDataContext _context;

    public ComplianceDocumentsListQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<InfoList<ComplianceDocumentsItem>>> Handle(ComplianceDocumentsListQuery query, CancellationToken cancellationToken)
    {
        var client = await _context.Clients.FirstAsync(x => x.Id == query.ClientId, cancellationToken);
        var documentIds = client.Documents.Select(x => x.DocumentId).ToList();
        var documents = await _context.Documents.Where(x => documentIds.Contains(x.Id)).ToListAsync(cancellationToken);

        var response = (from d in documents
                        join cd in client.Documents on d.Id equals cd.DocumentId
                        select new ComplianceDocumentsItem
                        {
                            Id = d.Id,
                            DocumentType = cd.DocumentType,
                            OriginalFileName = d.FileName,
                            UpdatedAt = d.CreatedAt
                        }).ToList();

        var infoList = new InfoList<ComplianceDocumentsItem>(response);

        return Result.Ok(infoList);
    }
}