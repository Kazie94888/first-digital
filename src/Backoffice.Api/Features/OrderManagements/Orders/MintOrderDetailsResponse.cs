using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Orders;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Orders;

internal sealed record MintOrderDetailsResponse
{
    public required List<MintProgressOverview> Overview { get; init; }
    public required MintOrderSummary Summary { get; init; }
    public MintBankDetails? BankDetails { get; internal set; }
    public MintRsnDetails? RsnDetails { get; internal set; }
    public required MintingDetails MintingDetails { get; init; }
}

internal sealed record MintProgressOverview
{
    public required MintProgressOverviewStatus Status { get; init; }
    public required string Title { get; init; }
    public MoneyDto? Amount { get; init; }
}

internal sealed record MintProgressOverviewTitle
{
    public const string Ordered = "Ordered";
    public const string Deposited = "Deposited";
    public const string Minted = "Minted";
}

internal enum MintProgressOverviewStatus
{
    Incomplete,
    Upcoming,
    Completed
}

internal sealed record MintOrderSummary
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

internal sealed record MintBankDetails
{
    public required string FromBankAccountName { get; init; }
    public required string FromIban { get; init; }

    public required string ToBankAccountName { get; init; }
    public required string ToSwift { get; init; }
    public required string ToIban { get; init; }
    public required string ToBankBranchAddress { get; init; }
    public string? ReferenceNumber { get; init; }
}

internal sealed record MintRsnDetails
{
    public FdtAccountDto? FromFdtAccount { get; init; }
    public FdtAccountDto? ToFdtAccount { get; init; }
    public string? TransactionReferenceNumber { get; init; }
}

internal sealed record MintingDetails
{
    public MoneyDto? DepositAmount { get; init; }
    public MoneyDto? MintingAmount { get; internal set; }

    public required string TransactionHash { get; init; }
    public required BlockchainNetwork Network { get; init; }
    public string? FromWalletAddress { get; init; }
    public required string ToWalletAddress { get; init; }
    public required int ToWalletUsageCount { get; init; }
}