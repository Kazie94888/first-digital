using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Application;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.AuthorizedUsers;

public sealed record AuthorizedUsersQuery : PagedListQuery, IQuery<InfoPagedList<AuthorizedUsersResponse>>
{
    [FromRoute(Name = "clientId")]
    public ClientId ClientId { get; init; }

    [FromQuery(Name = "onlyArchived")]
    public bool OnlyArchived { get; init; }
}