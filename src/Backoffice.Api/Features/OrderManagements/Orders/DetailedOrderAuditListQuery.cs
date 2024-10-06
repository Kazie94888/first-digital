using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.AuditLogs;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Domain.Orders;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Orders;

public sealed record DetailedOrderAuditListQuery : IQuery<InfoList<DetailedOrderAuditListQueryResponse>>
{
    [FromRoute(Name = "orderId")]
    public required OrderId OrderId { get; init; }
};

public sealed record DetailedOrderAuditListQueryResponse
{
    public required AuditLogId Id { get; init; }
    public required string Event { get; init; }
    public required string Description { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}