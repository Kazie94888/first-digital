using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Overview;

public sealed record ClientOrderOverviewListQuery : IQuery<InfoList<ClientOrderOverviewListResponse>>
{
    [FromRoute(Name = "clientId")]
    public ClientId ClientId { get; init; }
}
