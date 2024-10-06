using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Deposit.Events;

public abstract class DepositFdtAccountAuditEventBase : AuditEventBase
{
    protected DepositFdtAccountAuditEventBase(DepositFdtAccountId depositFdtAccountId, UserInfo auditedBy) : base(auditedBy)
    {
        Parameters.Add(new AuditLogs.AuditLogParameter(nameof(DepositFdtAccountId), depositFdtAccountId.Value.ToString()));
    }
}