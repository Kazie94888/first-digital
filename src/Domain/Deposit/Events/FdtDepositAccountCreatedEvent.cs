using SmartCoinOS.Domain.AuditLogs;
using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Deposit.Events;

public sealed class FdtDepositAccountCreatedEvent : DepositFdtAccountAuditEventBase
{
    public FdtDepositAccountCreatedEvent(DepositFdtAccountId id, string fdtDepositAccountName, FdtDepositAccountNumber fdtDepositAccountNumber, UserInfo auditedBy) : base(id, auditedBy)
    {
        FdtDepositAccountNumber = fdtDepositAccountNumber;
        FdtDepositAccountName = fdtDepositAccountName;

        Parameters.Add(new AuditLogParameter(nameof(FdtDepositAccountNumber), fdtDepositAccountNumber));
        Parameters.Add(new AuditLogParameter(nameof(FdtDepositAccountName), fdtDepositAccountName));
    }

    public FdtDepositAccountNumber FdtDepositAccountNumber { get; }
    public string FdtDepositAccountName { get; }

    public override string GetDescription()
    {
        return $"Deposit FDT Account '{FdtDepositAccountNumber}' created by '{AuditedBy.Username}'";
    }
}
