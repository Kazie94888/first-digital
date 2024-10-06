using SmartCoinOS.ClientPortal.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Orders;

namespace SmartCoinOS.ClientPortal.Api.Features.CompanyInformation.Wallets;

internal sealed record WalletDetailsResponse
{
    public required WalletDetails Wallet { get; init; }
    public required List<RelatedOrdersDto> RelatedOrders { get; init; }
}

internal sealed record WalletDetails
{
    public required WalletId Id { get; init; }
    public string? Alias { get; init; }
    public required BlockchainAddressDto Address { get; init; }
    public required EntityVerificationStatus Status { get; init; }
    public required bool Archived { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required UserInfoDto CreatedBy { get; init; }
    public List<WalletTotals> Totals { get; internal set; } = [];
}

internal sealed record WalletTotals
{
    public required OrderType Type { get; init; }
    public required MoneyDto Amount { get; init; }
}
