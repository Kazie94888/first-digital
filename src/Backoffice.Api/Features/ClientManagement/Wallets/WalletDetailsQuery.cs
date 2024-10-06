using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Wallets;

public sealed record WalletDetailsQuery : IQuery<WalletDetailsResponse>
{
    [FromRoute(Name = "clientId")]
    public required ClientId ClientId { get; init; }
    [FromRoute(Name = "walletId")]
    public required WalletId WalletId { get; init; }
}