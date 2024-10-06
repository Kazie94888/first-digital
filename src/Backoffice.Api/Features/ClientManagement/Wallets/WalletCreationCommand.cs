using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Wallets;

public sealed record WalletCreationCommand : ICommand<EntityId>
{
    [FromRoute(Name = "clientId")]
    public required ClientId ClientId { get; init; }
    public required UserInfoDto UserInfo { get; init; }

    [FromBody]
    public required CreateWallet Wallet { get; init; }
}

public sealed record CreateWallet
{
    public required string Address { get; init; }
    public required BlockchainNetwork Network { get; init; }
    public string? Alias { get; init; }
}
