using SmartCoinOS.Domain.AuditLogs;
using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Clients.Events;

public sealed class ClientDormantEvent : ClientAuditEventBase
{
    public ClientDormantEvent(string legalEntityName, ClientStatus oldStatus, ClientId clientId, UserInfo actor) : base(clientId, actor)
    {
        Parameters.Add(new AuditLogParameter(nameof(oldStatus), oldStatus.ToString()));
        Parameters.Add(new AuditLogParameter(nameof(legalEntityName), legalEntityName));
        LegalEntityName = legalEntityName;
    }

    public string LegalEntityName { get; }

    public override string GetDescription()
    {
        return $"{LegalEntityName} moved to status Doormant by {AuditedBy.Username}";
    }
}
