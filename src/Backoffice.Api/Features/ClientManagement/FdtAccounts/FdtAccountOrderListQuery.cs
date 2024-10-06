using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Application;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.FdtAccounts;

public sealed record FdtAccountOrderListQuery : PagedListQuery, IQuery<InfoPagedList<FdtAccountOrderListResponse>>
{
    [FromRoute(Name = "clientId")]
    public required ClientId ClientId { get; init; }

    [FromRoute(Name = "fdtAccountId")]
    public FdtAccountId FdtAccountId { get; init; }
}
