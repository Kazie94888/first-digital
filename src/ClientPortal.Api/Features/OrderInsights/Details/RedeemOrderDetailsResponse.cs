using SmartCoinOS.ClientPortal.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Orders;

namespace SmartCoinOS.ClientPortal.Api.Features.OrderInsights.Details;

internal sealed record RedeemOrderDetailsResponse
{
    public required List<RedeemProgressOverview> Overview { get; init; }
    public required RedeemOrderSummary Summary { get; init; }
    public required RedeemDepositDetails DepositDetails { get; init; }
    public required RedeemBurningDetails BurningDetails { get; init; }
    public RedeemBankDetails? BankDetails { get; internal set; }
    public RedeemRsnDetails? RsnDetails { get; internal set; }
}

internal sealed record RedeemProgressOverview
{
    public required RedeemProgressOverviewStatus Status { get; init; }
    public required string Title { get; init; }
    public MoneyDto? Amount { get; init; }
}

internal sealed record RedeemProgressOverviewTitle
{
    public const string TransferTokens = "Transfer tokens";
    public const string Burn = "Burn";
    public const string ReceiptOfFunds = "Receipt of funds";
}

internal enum RedeemProgressOverviewStatus
{
    Incomplete,
    InProgress,
    Completed
}

internal sealed record RedeemOrderSummary
{
    public required string OrderNumber { get; init; }
    public required OrderStatus OrderStatus { get; init; }
    public OrderProcessingStatus? OrderProcessingStatus { get; init; }
    public required ClientId ClientId { get; init; }
    public required string ClientName { get; init; }
    public required ClientStatus ClientStatus { get; init; }
    public required MoneyDto OrderedAmount { get; init; }
    public required BlockchainNetwork Network { get; init; }
}

internal sealed record RedeemDepositDetails
{
    public string? TransactionHash { get; init; }
    public required BlockchainNetwork Network { get; init; }
    public required string FromAddress { get; init; }
    public required string ToAddress { get; init; }
}

internal sealed record RedeemBurningDetails
{
    public required MoneyDto DepositAmount { get; init; }
    public MoneyDto? BurningAmount { get; init; }
    public required BlockchainAddressDto FromWallet { get; init; }
    public required int FromWalletUsageCount { get; init; }
    public string? TransactionHash { get; init; }
}

internal sealed record RedeemBankDetails
{
    public required string FromBankAccountName { get; init; }
    public required string FromIban { get; init; }
    public required string ToBankAccountName { get; init; }
    public required string ToSwift { get; init; }
    public required string ToIban { get; init; }
    public required string ToBankBranchAddress { get; init; }
}

internal sealed record RedeemRsnDetails
{
    public required FdtAccountDto FromFdtAccount { get; init; }
    public required FdtAccountDto ToFdtAccount { get; init; }
    public string? TransactionReferenceNumber { get; init; }
}
