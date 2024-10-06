using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Minting;

public sealed record MintOrderCommand : ICommand<EntityId>
{
    [FromBody]
    public required MintOrderRequest Request { get; init; }
    public required UserInfoDto UserInfo { get; init; }
}

public sealed record MintOrderRequest
{
    public required ClientId ClientId { get; init; }
    public required MoneyDto MintAmount { get; init; }
    public BankAccountId? BankAccountId { get; init; }
    public FdtAccountId? FdtAccountId { get; init; }
    public required WalletId WalletId { get; init; }
}
