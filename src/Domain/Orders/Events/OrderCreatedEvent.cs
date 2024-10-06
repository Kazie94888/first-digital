using SmartCoinOS.Domain.AuditLogs;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Exceptions;
using SmartCoinOS.Domain.SeedWork;


namespace SmartCoinOS.Domain.Orders.Events;

public sealed class OrderCreatedEvent : OrderAuditEventBase
{
    public OrderCreatedEvent(OrderId orderId, OrderNumber orderNumber, WalletId clientWalletId, OrderType orderType,
        ClientId clientId, UserInfo userInfo) : base(orderId, clientId, userInfo)
    {
        Parameters.Add(new AuditLogParameter(nameof(Order.OrderNumber), orderNumber.Value));
        Parameters.Add(new AuditLogParameter(nameof(Order.Type), orderType.ToString()));

        OrderType = orderType;
        UserInfo = userInfo;
        ClientWalletId = clientWalletId;
        ClientId = clientId;
    }

    public OrderType OrderType { get; }
    public UserInfo UserInfo { get; }
    public WalletId ClientWalletId { get; }
    public ClientId ClientId { get; }

    public override string GetDescription()
    {
        return OrderType switch
        {
            OrderType.Mint => $"Mint order created by {UserInfo.Username}",
            OrderType.Redeem => $"Redeem order created by {UserInfo.Username}",

            _ => throw new CreateEntityException($"{nameof(OrderType)} is not a known order type"),
        };
    }
}
