using SmartCoinOS.Domain.AuditLogs;
using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Clients.Events;

public sealed class FdtAccountCreatedEvent : ClientAuditEventBase
{
    public FdtAccountCreatedEvent(FdtAccountNumber fdtAccountNumber, string fdtClientName, ClientId clientId, UserInfo auditedBy) : base(clientId, auditedBy)
    {
        FdtAccountNumber = fdtAccountNumber;
        FdtClientName = fdtClientName;

        Parameters.Add(new AuditLogParameter(nameof(FdtAccountNumber), fdtAccountNumber));
        Parameters.Add(new AuditLogParameter(nameof(FdtClientName), fdtAccountNumber));
    }

    public FdtAccountNumber FdtAccountNumber { get; }
    public string FdtClientName { get; }

    public override string GetDescription()
    {
        return $"FDT Account '{FdtAccountNumber}' created by '{AuditedBy.Username}'";
    }
}
