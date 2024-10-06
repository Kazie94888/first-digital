using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.Settings.DepositBanks;

public sealed record CreateDepositBankCommand : ICommand<EntityId>
{
    [FromBody]
    public required CreateDepositBank Request { get; init; }
    public required UserInfoDto UserInfo { get; init; }
}

public sealed record CreateDepositBank
{
    public required DepositBankAccount DepositBankAccount { get; init; }
    public required DepositBankAddress DepositBankAddress { get; init; }
}

public sealed record DepositBankAccount
{
    public required string Beneficiary { get; init; }
    public required string Iban { get; init; }
    public required string BankName { get; init; }
    public required string SwiftCode { get; init; }
}

public sealed record DepositBankAddress
{
    public required string Street { get; init; }
    public required string PostalCode { get; init; }
    public required string City { get; init; }
    public string? State { get; init; }
    public required string Country { get; init; }
}