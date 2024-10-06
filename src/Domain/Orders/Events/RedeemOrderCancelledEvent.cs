using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Orders.Events;

public sealed class RedeemOrderCancelledEvent : OrderAuditEventBase
{
    public RedeemOrderCancelledEvent(OrderId orderId, OrderNumber orderNumber, string reason, ClientId clientId, UserInfo cancelledBy)
        : base(orderId, clientId, cancelledBy)
    {
        OrderNumber = orderNumber;
        Reason = reason;

        Parameters.Add(new AuditLogs.AuditLogParameter(nameof(OrderNumber), OrderNumber.ToString()));
        Parameters.Add(new AuditLogs.AuditLogParameter(nameof(Reason), reason));
    }

    public OrderNumber OrderNumber { get; }
    public string Reason { get; }

    public override string GetDescription()
    {
        return $"Redeem #{OrderNumber} cancelled ({Reason})";
    }
}