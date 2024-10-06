using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.ClientPortal.Api.Base.Dto;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Domain.Shared;

namespace SmartCoinOS.ClientPortal.Api.Features.CompanyInformation.Documents;

internal sealed record DocumentListQuery : IQuery<InfoList<DocumentListResponseItem>>
{
    public required UserInfoDto UserInfo { get; init; }
}

internal sealed record DocumentListResponseItem
{
    public required DocumentId DocumentId { get; init; }
    public required string OriginalFileName { get; init; }
    public required string DocumentType { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }
}