using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Deposit;

namespace SmartCoinOS.Backoffice.Api.Features.Settings.DepositWallets;

public sealed record DefaultDepositWalletCommand : ICommand<EntityId>
{
    [FromRoute(Name = "id")]
    public required DepositWalletId Id { get; init; }

    public required UserInfoDto UserInfo { get; init; }
}