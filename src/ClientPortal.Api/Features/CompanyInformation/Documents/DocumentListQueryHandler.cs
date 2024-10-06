using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Persistence;

namespace SmartCoinOS.ClientPortal.Api.Features.CompanyInformation.Documents;

internal sealed class DocumentListQueryHandler : IQueryHandler<DocumentListQuery, InfoList<DocumentListResponseItem>>
{
    private readonly ReadOnlyDataContext _dataContext;

    public DocumentListQueryHandler(ReadOnlyDataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<Result<InfoList<DocumentListResponseItem>>> Handle(DocumentListQuery query, CancellationToken cancellationToken)
    {
        var documents = await _dataContext.Documents
            .Where(doc => doc.ClientId.HasValue && doc.ClientId.Value == query.UserInfo.ClientId)
            .Select(d => new DocumentListResponseItem
            {
                DocumentId = d.Id,
                OriginalFileName = d.FileName,
                DocumentType = d.ClientDocumentType ?? string.Empty,
                UpdatedAt = d.CreatedAt
            })
            .OrderByDescending(d => d.UpdatedAt)
            .ToListAsync(cancellationToken);

        return Result.Ok(new InfoList<DocumentListResponseItem>(documents));
    }
}