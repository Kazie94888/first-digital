using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Application;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Orders;

public sealed record ClientOrderListQuery : PagedListFiqlQuery, IQuery<InfoPagedList<ClientOrderListResponse>>
{
    [FromRoute(Name = "clientId")]
    public required ClientId ClientId { get; init; }
}