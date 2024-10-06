using SmartCoinOS.Domain.AuditLogs;
using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Clients.Events;

public sealed class ClientDeactivatedEvent : ClientAuditEventBase
{
    public ClientDeactivatedEvent(string legalEntityName, ClientStatus oldStatus, ClientId clientId, UserInfo auditedBy) : base(clientId, auditedBy)
    {
        Parameters.Add(new AuditLogParameter(nameof(oldStatus), oldStatus.ToString()));
        Parameters.Add(new AuditLogParameter(nameof(legalEntityName), legalEntityName));

        LegalEntityName = legalEntityName;
    }

    public string LegalEntityName { get; }

    public override string GetDescription()
    {
        return $"Client '{LegalEntityName}' was deactivated by {AuditedBy.Username}";
    }
}