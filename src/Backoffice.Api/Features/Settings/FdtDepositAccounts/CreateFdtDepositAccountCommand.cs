using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.Settings.FdtDepositAccounts;

public sealed record CreateFdtDepositAccountCommand : ICommand<EntityId>
{
    [FromBody]
    public required CreateFdtDepositAccountRequest FdtDepositAccount { get; init; }

    public required UserInfoDto UserInfo { get; init; }
}

public sealed record CreateFdtDepositAccountRequest
{
    public required string AccountName { get; init; }
    public required string AccountNumber { get; init; }
}
