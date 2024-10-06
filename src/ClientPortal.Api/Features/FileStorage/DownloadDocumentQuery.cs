using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.ClientPortal.Api.Base.Dto;
using SmartCoinOS.Domain.Shared;

namespace SmartCoinOS.ClientPortal.Api.Features.FileStorage;

internal sealed record DownloadDocumentQuery : IQuery<FileDto>
{
    [FromRoute(Name = "documentId")]
    public required DocumentId DocumentId { get; init; }

    public required UserInfoDto UserInfo { get; init; }
}