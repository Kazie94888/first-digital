using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Deposit;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Overview;

public sealed record ChangeDepositBankCommand : ICommand<ChangeDepositBankResponse>
{
    [FromRoute(Name = "clientId")]
    public required ClientId ClientId { get; init; }

    [FromRoute(Name = "depositBankId")]
    public required DepositBankId DepositBankId { get; init; }

    public required UserInfoDto UserInfo { get; init; }
}

public sealed record ChangeDepositBankResponse
{
    public required DepositBankId Id { get; init; }
    public required string BankName { get; init; }
    public required string Iban { get; init; }
}