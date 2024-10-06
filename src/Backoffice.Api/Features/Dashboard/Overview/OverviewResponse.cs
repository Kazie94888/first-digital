using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Application.Enums;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Domain.Orders;

namespace SmartCoinOS.Backoffice.Api.Features.Dashboard.Overview;

internal sealed record OverviewResponse
{
    public required IReadOnlyList<OverviewAggregateResponse> Totals { get; init; }
    public required InfoList<OverviewOrderInProgress> OrdersInProgress { get; init; }
    public required InfoList<OverviewApplicationsInProgress> ApplicationInProgress { get; init; }
}

internal sealed record OverviewAggregateResponse
{
    public required MoneyDto MintInProgress { get; init; }
    public required MoneyDto MintCompleted { get; init; }
    public required MoneyDto RedeemInProgress { get; init; }
    public required MoneyDto RedeemCompleted { get; init; }
}

internal sealed record OverviewOrderInProgress
{
    public OrderId Id { get; init; }
    public required string PrettyId { get; init; }
    public OrderType OrderType { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public required string ClientName { get; init; }
    public OrderStatus Progress { get; init; }
    public OrderProcessingStatus? ProcessingStatus { get; init; }
    public required MoneyDto AmountOrdered { get; init; }
}

internal sealed record OverviewApplicationsInProgress
{
    public required ApplicationId Id { get; init; }
    public required string LegalEntityName { get; init; }
    public required UserInfoDto CreatedBy { get; init; }
    public required string PrettyId { get; init; }
    public required ApplicationStatus Status { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}
