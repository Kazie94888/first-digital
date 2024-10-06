using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Wallets;

internal sealed record DeclineWalletCommand : ICommand<EntityId>
{
    [FromRoute(Name = "clientId")]
    public ClientId ClientId { get; init; }

    [FromRoute(Name = "walletId")]
    public WalletId WalletId { get; init; }

    public required UserInfoDto UserInfo { get; init; }
}
