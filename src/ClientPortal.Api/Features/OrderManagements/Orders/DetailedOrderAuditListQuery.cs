using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.ClientPortal.Api.Base.Dto;
using SmartCoinOS.Domain.AuditLogs;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Domain.Orders;

namespace SmartCoinOS.ClientPortal.Api.Features.OrderManagements.Orders;

internal sealed record DetailedOrderAuditListQuery : IQuery<InfoList<OrderAuditItem>>
{
    [FromRoute(Name = "orderId")]
    public required OrderId OrderId { get; init; }
    
    public required UserInfoDto UserInfo { get; init; }
};

public sealed record OrderAuditItem
{
    public required AuditLogId Id { get; init; }
    public required string Event { get; init; }
    public required string Description { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}