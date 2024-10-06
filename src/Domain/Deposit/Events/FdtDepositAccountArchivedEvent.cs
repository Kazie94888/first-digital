using SmartCoinOS.Domain.AuditLogs;
using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Deposit.Events;

public sealed class FdtDepositAccountArchivedEvent : DepositFdtAccountAuditEventBase
{
    public FdtDepositAccountArchivedEvent(DepositFdtAccountId id, FdtDepositAccountNumber fdtDepositAccountNumber, UserInfo auditedBy) : base(id, auditedBy)
    {
        FdtDepositAccountNumber = fdtDepositAccountNumber;

        Parameters.Add(new AuditLogParameter(nameof(FdtDepositAccountNumber), fdtDepositAccountNumber));
    }

    public FdtDepositAccountNumber FdtDepositAccountNumber { get; }

    public override string GetDescription()
    {
        return $"FDT Deposit Account '{FdtDepositAccountNumber}' was archived by {AuditedBy.Username}";
    }
}