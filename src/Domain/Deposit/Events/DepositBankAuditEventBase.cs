using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Deposit.Events;

public abstract class DepositBankAuditEventBase : AuditEventBase
{
    protected DepositBankAuditEventBase(DepositBankId depositBankId, UserInfo auditedBy) : base(auditedBy)
    {
        Parameters.Add(new AuditLogs.AuditLogParameter(nameof(DepositBankId), depositBankId.Value.ToString()));
    }
}
