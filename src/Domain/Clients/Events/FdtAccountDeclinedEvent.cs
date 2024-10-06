using SmartCoinOS.Domain.AuditLogs;
using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Clients.Events;

public sealed class FdtAccountDeclinedEvent : ClientAuditEventBase
{
    public FdtAccountDeclinedEvent(ClientId clientId, UserInfo declinedBy, FdtAccountNumber fdtAccountNumber) : base(clientId, declinedBy)
    {
        FdtAccountNumber = fdtAccountNumber;

        Parameters.Add(new AuditLogParameter(nameof(FdtAccountNumber), fdtAccountNumber));
    }

    public FdtAccountNumber FdtAccountNumber { get; }

    public override string GetDescription()
    {
        return $"FDT Account '{FdtAccountNumber}' was declined by {AuditedBy.Username}";
    }
}