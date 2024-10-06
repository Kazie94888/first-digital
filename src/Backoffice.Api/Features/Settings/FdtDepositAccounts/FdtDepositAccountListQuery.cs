using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Domain.Deposit;

namespace SmartCoinOS.Backoffice.Api.Features.Settings.FdtDepositAccounts;

public sealed record FdtDepositAccountListQuery : IQuery<InfoList<FdtDepositAccountListItem>>;

public sealed record FdtDepositAccountListItem
{
    public required DepositFdtAccountId Id { get; init; }
    public required string AccountName { get; init; }
    public required string AccountNumber { get; init; }
    public required bool Archived { get; init; }
    public DateTimeOffset? ArchivedAt { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}