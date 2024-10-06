using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Orders;

public sealed record ClientOrderTotalsListQuery : IQuery<List<ClientOrderTotalsListResponse>>
{
    [FromRoute(Name = "clientId")]
    public ClientId ClientId { get; init; }
}
