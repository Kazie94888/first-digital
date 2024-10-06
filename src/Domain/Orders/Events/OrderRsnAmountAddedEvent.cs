using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.SeedWork;


namespace SmartCoinOS.Domain.Orders.Events;

public sealed class OrderRsnAmountAddedEvent : OrderAuditEventBase
{
    public OrderRsnAmountAddedEvent(OrderId orderId, ClientId clientId, UserInfo userInfo) : base(orderId, clientId, userInfo)
    {
    }

    public override string GetDescription()
    {
        return "RSN amount added.";
    }
}

