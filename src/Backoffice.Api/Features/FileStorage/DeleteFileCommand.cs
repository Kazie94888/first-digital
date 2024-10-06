using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Shared;

namespace SmartCoinOS.Backoffice.Api.Features.FileStorage;

public sealed class DeleteFileCommand : ICommand<EntityId>
{
    [FromBody]
    public required DeleteFileRequest Request { get; init; }
}

public sealed class DeleteFileRequest
{
    public required DocumentId Id { get; init; }
}