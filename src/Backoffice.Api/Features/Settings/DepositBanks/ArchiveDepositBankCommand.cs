using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Deposit;

namespace SmartCoinOS.Backoffice.Api.Features.Settings.DepositBanks;

public sealed record ArchiveDepositBankCommand : ICommand<EntityId>
{
    [FromRoute(Name = "id")]
    public required DepositBankId Id { get; init; }

    public required UserInfoDto UserInfo { get; init; }
}
