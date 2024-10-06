using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Redeeming;

public sealed record RedeemOrderCommand : ICommand<EntityId>
{
    [FromBody]
    public required RedeemOrder Request { get; init; }
    public required UserInfoDto UserInfo { get; init; }
}

public sealed record RedeemOrder
{
    public required ClientId ClientId { get; init; }
    public required MoneyDto RedeemAmount { get; init; }
    public BankAccountId? BankAccountId { get; init; }
    public FdtAccountId? FdtAccountId { get; init; }
    public required WalletId WalletId { get; init; }
}
