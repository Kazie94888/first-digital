namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Orders;

public sealed record OrderAuditListResponse
{
    public DateOnly Date { get; init; }
    public List<OrderAuditItem> Data { get; init; } = [];
}

public sealed record OrderAuditItem
{
    public string? Type { get; init; }
    public string? Id { get; init; }
    public string? Description { get; init; }
    public DateTimeOffset Date { get; init; }
}