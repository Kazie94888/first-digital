using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.ClientPortal.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.ClientPortal.Api.Features.OrderManagements.Minting;

internal sealed record MintOrderCommand : ICommand<EntityId>
{
    [FromBody]
    public required MintOrderRequest Request { get; init; }

    public required UserInfoDto UserInfo { get; init; }
}

internal sealed record MintOrderRequest
{
    public required MoneyDto MintAmount { get; init; }
    public BankAccountId? BankAccountId { get; init; }
    public FdtAccountId? FdtAccountId { get; init; }
    public required WalletId WalletId { get; init; }
}