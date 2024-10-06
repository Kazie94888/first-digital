using SmartCoinOS.Domain.AuditLogs;
using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Clients.Events;

public sealed class FdtAccountVerifiedEvent : ClientAuditEventBase
{
    public FdtAccountVerifiedEvent(ClientId clientId, UserInfo verifiedBy, FdtAccountNumber fdtAccountNumber) : base(clientId, verifiedBy)
    {
        FdtAccountNumber = fdtAccountNumber;

        Parameters.Add(new AuditLogParameter(nameof(FdtAccountNumber), fdtAccountNumber));
    }

    public FdtAccountNumber FdtAccountNumber { get; }

    public override string GetDescription()
    {
        return $"FDT Account '{FdtAccountNumber}' was verified by {AuditedBy.Username}";
    }
}