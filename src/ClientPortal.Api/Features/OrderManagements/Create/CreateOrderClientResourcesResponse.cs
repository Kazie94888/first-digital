using SmartCoinOS.ClientPortal.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.ClientPortal.Api.Features.OrderManagements.Create;

internal sealed record CreateOrderClientResourcesResponse
{
    public required List<BankAccountListItem> BankAccounts { get; init; }
    public required List<FdtAccountListItem> FdtAccounts { get; init; }
    public required List<WalletListItem> Wallets { get; init; }
}

internal sealed record BankAccountListItem
{
    public required BankAccountId Id { get; init; }
    public required string Name { get; init; }
    public required string Iban { get; init; }
    public string? Alias { get; init; }
}

internal sealed record WalletListItem
{
    public required WalletId Id { get; init; }
    public required BlockchainAddressDto Address { get; init; }
    public string? Alias { get; init; }
}

internal sealed record FdtAccountListItem
{
    public required FdtAccountId Id { get; init; }
    public required FdtAccountDto Account { get; init; }
    public string? Alias { get; init; }
}
