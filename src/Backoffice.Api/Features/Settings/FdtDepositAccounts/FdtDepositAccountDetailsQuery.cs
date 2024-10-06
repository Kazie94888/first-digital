using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Deposit;

namespace SmartCoinOS.Backoffice.Api.Features.Settings.FdtDepositAccounts;

public class FdtDepositAccountDetailsQuery : IQuery<FdtDepositAccountDetailsResponse>
{
    [FromRoute(Name = "id")]
    public required DepositFdtAccountId Id { get; init; }
}