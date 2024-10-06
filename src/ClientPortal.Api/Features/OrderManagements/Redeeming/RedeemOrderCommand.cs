using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.ClientPortal.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.ClientPortal.Api.Features.OrderManagements.Redeeming;

internal sealed class RedeemOrderCommand : ICommand<EntityId>
{
    [FromBody]
    public required RedeemOrder Request { get; init; }
    
    public required UserInfoDto UserInfo { get; init; }
}

internal sealed record RedeemOrder
{
    public required MoneyDto RedeemAmount { get; init; }
    public required WalletId WalletId { get; init; }
    
    public BankAccountId? BankAccountId { get; init; }
    public FdtAccountId? FdtAccountId { get; init; }
}