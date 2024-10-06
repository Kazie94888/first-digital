using FluentResults;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Services;

namespace SmartCoinOS.ClientPortal.Api.Features.FileStorage;

internal sealed class DocumentTypeListQueryHandler : IQueryHandler<DocumentTypeListQuery, Dictionary<string, IReadOnlyList<string>>>
{
    public Task<Result<Dictionary<string, IReadOnlyList<string>>>> Handle(DocumentTypeListQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(Result.Ok(DocumentTypeService.Instance.GetAll()));
    }
}