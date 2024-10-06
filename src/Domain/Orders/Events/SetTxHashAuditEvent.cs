using SmartCoinOS.Domain.AuditLogs;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.SeedWork;


namespace SmartCoinOS.Domain.Orders.Events;

public sealed class SetTxHashAuditEvent : OrderAuditEventBase
{
    public SetTxHashAuditEvent(OrderId orderId, string txHash, ClientId clientId, UserInfo auditedBy) : base(orderId, clientId, auditedBy)
    {
        Parameters.Add(new AuditLogParameter(nameof(txHash), txHash));
    }

    public override string GetDescription()
    {
        return $"{AuditedBy.Username} set transaction hash";
    }
}
