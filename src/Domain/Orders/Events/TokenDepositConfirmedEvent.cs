using SmartCoinOS.Domain.AuditLogs;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.SeedWork;


namespace SmartCoinOS.Domain.Orders.Events;

public sealed class TokenDepositConfirmedEvent : OrderAuditEventBase
{
    public TokenDepositConfirmedEvent(OrderId orderId, Money amount, ClientId clientId, UserInfo auditedBy) : base(orderId, clientId, auditedBy)
    {
        Parameters.Add(new AuditLogParameter(nameof(Money.Amount), amount.Amount.ToString("G")));
        Parameters.Add(new AuditLogParameter(nameof(Money.Currency), amount.Currency));
    }

    public override string GetDescription()
    {
        return $"Deposit confirmed by {AuditedBy.Username}";
    }
}