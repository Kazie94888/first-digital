using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Deposit;

namespace SmartCoinOS.Backoffice.Api.Features.Settings.DepositBanks;

public sealed record DepositBankDetailsQuery : IQuery<DepositBankDetails>
{
    [FromRoute(Name = "id")]
    public required DepositBankId Id { get; init; }
}

public sealed record DepositBankDetails
{
    public required DepositBankId Id { get; init; }
    public required string Name { get; init; }
    public required string Beneficiary { get; init; }
    public required string Swift { get; init; }
    public required string Iban { get; init; }
    public required string Address { get; init; }
    public required bool Archived { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required bool Default { get; init; }
}
