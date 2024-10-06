using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.SeedWork;


namespace SmartCoinOS.Domain.Orders.Events;

public sealed class RedeemPaymentInstructionConfirmedEvent : OrderAuditEventBase
{
    public RedeemPaymentInstructionConfirmedEvent(OrderId orderId, ClientId clientId, UserInfo userInfo) : base(orderId, clientId, userInfo)
    {
    }

    public override string GetDescription()
    {
        return "Payment Instruction Confirmed";
    }
}