using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.SeedWork;


namespace SmartCoinOS.Domain.Orders.Events;

public sealed class RedeemOrderCompletedEvent : OrderAuditEventBase
{
    public RedeemOrderCompletedEvent(OrderId orderId, ClientId clientId, UserInfo auditedBy) : base(orderId, clientId, auditedBy)
    {
    }

    public override string GetDescription()
    {
        return $"Redeem Order Completed";
    }
}
