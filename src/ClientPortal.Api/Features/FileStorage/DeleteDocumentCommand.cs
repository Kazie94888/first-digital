using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.ClientPortal.Api.Base.Dto;
using SmartCoinOS.Domain.Shared;

namespace SmartCoinOS.ClientPortal.Api.Features.FileStorage;

internal sealed class DeleteDocumentCommand : ICommand<EntityId>
{
    [FromBody]
    public required DeleteFileRequest Request { get; init; }
    
    public required UserInfoDto UserInfo { get; init; }
}

internal sealed class DeleteFileRequest
{
    public required DocumentId Id { get; init; }
}