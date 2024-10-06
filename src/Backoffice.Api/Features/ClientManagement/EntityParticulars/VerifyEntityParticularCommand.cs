using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.EntityParticulars;

internal sealed record VerifyEntityParticularCommand : ICommand<EntityId>
{
    [FromRoute(Name = "clientId")]
    public ClientId ClientId { get; init; }

    [FromRoute(Name = "entityParticularId")]
    public EntityParticularId EntityParticularId { get; init; }

    public required UserInfoDto UserInfo { get; init; }
}
