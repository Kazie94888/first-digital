using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Shared;

namespace SmartCoinOS.Backoffice.Api.Features.FileStorage;

public sealed record UploadFileCommand : ICommand<UploadFileResponse>
{
    [FromForm]
    public required IFormFile FormFile { get; init; }
    public required UserInfoDto UserInfo { get; init; }
}

public sealed record UploadFileResponse
{
    public required DocumentId Id { get; init; }
    public required string Name { get; init; }
}
