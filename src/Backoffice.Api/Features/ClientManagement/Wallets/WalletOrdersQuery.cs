using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Application;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Wallets;

public sealed record WalletOrdersQuery : PagedListFiqlQuery, IQuery<InfoPagedList<WalletOrdersResponse>>
{
    [FromRoute(Name = "clientId")]
    public required ClientId ClientId { get; init; }
    [FromRoute(Name = "walletId")]
    public required WalletId WalletId { get; init; }
}