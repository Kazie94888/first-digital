using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Clients;

public sealed record ChangeClientStatusCommand : ICommand<EntityId>
{
    public required ClientId ClientId { get; init; }
    public required ClientStatus Status { get; init; }
    public required UserInfoDto UserInfo { get; init; }
}

public sealed record ChangeStatusRequest
{
    [FromBody]
    public required ClientStatus Status { get; init; }
}
