using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.ClientPortal.Api.Base.Dto;
using SmartCoinOS.Domain.Shared;

namespace SmartCoinOS.ClientPortal.Api.Features.FileStorage;

internal sealed record UploadDocumentCommand : ICommand<UploadDocumentResponse>
{
    [FromForm]
    public required IFormFile FormFile { get; init; }

    public required UserInfoDto UserInfo { get; init; }
}

internal sealed record UploadDocumentResponse
{
    public required DocumentId Id { get; init; }
    public required string Name { get; init; }
}