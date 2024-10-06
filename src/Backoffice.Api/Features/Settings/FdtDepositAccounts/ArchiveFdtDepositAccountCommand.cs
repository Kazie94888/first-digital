using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Deposit;

namespace SmartCoinOS.Backoffice.Api.Features.Settings.FdtDepositAccounts;

public sealed record ArchiveFdtDepositAccountCommand : ICommand<EntityId>
{
    [FromRoute(Name = "id")]
    public required DepositFdtAccountId Id { get; init; }

    public required UserInfoDto UserInfo { get; init; }
}
