using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.AuthorizedUsers;

public sealed record AuthorizedUsersCountQuery : IQuery<AuthorizedUsersCountResponse>
{
    [FromRoute(Name = "clientId")]
    public ClientId ClientId { get; init; }
}