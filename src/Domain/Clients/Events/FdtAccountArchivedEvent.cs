using SmartCoinOS.Domain.AuditLogs;
using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Clients.Events;

public sealed class FdtAccountArchivedEvent : ClientAuditEventBase
{
    public FdtAccountArchivedEvent(ClientId clientId, UserInfo auditedBy, FdtAccountNumber fdtAccountNumber) : base(clientId, auditedBy)
    {
        FdtAccountNumber = fdtAccountNumber;

        Parameters.Add(new AuditLogParameter(nameof(FdtAccountNumber), fdtAccountNumber));
    }

    public FdtAccountNumber FdtAccountNumber { get; }

    public override string GetDescription()
    {
        return $"FDT Account '{FdtAccountNumber}' was archived by {AuditedBy.Username}";
    }
}