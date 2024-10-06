using SmartCoinOS.Domain.Deposit;
using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Backoffice.Api.Features.Settings.FdtDepositAccounts;

public sealed record FdtDepositAccountDetailsResponse
{
    public required DepositFdtAccountId Id { get; init; }
    public required string AccountName { get; init; }
    public required string AccountNumber { get; init; }
    public required bool Archived { get; init; }
    public DateTimeOffset? ArchivedAt { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required UserInfo CreatedBy { get; init; }
}