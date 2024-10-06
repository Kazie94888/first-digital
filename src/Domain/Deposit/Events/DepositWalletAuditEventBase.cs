using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Deposit.Events;

public abstract class DepositWalletAuditEventBase : AuditEventBase
{
    protected DepositWalletAuditEventBase(DepositWalletId depositWalletId, UserInfo auditedBy) : base(auditedBy)
    {
        Parameters.Add(new AuditLogs.AuditLogParameter(nameof(DepositWalletId), depositWalletId.Value.ToString()));
    }
}
