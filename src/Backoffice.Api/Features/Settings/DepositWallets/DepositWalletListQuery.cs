using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Deposit;

namespace SmartCoinOS.Backoffice.Api.Features.Settings.DepositWallets;

public sealed record DepositWalletListQuery : IQuery<InfoList<DepositWalletListItem>>;

public sealed record DepositWalletListItem
{
    public required DepositWalletId Id { get; init; }
    public required WalletAccount Address { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}
