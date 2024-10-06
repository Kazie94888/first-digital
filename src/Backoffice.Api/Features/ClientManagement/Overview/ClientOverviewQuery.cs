using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Overview;

public sealed record ClientOverviewQuery : IQuery<ClientOverviewResponse>
{
    [FromRoute(Name = "clientId")]
    public ClientId ClientId { get; init; }
}
