using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.AuthorizedUsers;

public sealed record ArchiveAuthorizedUserCommand : ICommand<EntityId>
{

    [FromRoute(Name = "clientId")]
    public required ClientId ClientId { get; init; }

    [FromRoute(Name = "userId")]
    public required AuthorizedUserId UserId { get; init; }

    public required UserInfoDto UserInfo { get; init; }
}
