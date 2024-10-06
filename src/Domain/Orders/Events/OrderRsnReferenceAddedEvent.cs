using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.SeedWork;


namespace SmartCoinOS.Domain.Orders.Events;

public sealed class OrderRsnReferenceAddedEvent : OrderAuditEventBase
{
    public OrderRsnReferenceAddedEvent(OrderId orderId, ClientId clientId, UserInfo userInfo) : base(orderId, clientId, userInfo)
    {
    }

    public override string GetDescription()
    {
        return "RSN Reference number added.";
    }
}

