using SmartCoinOS.Domain.AuditLogs;
using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Clients.Events;

public sealed class ClientCreatedAuditEvent : ClientAuditEventBase
{
    public ClientCreatedAuditEvent(string applicationNumber, string legalEntityName, ClientId clientId, UserInfo actor) : base(clientId, actor)
    {
        Parameters.Add(new AuditLogParameter(nameof(legalEntityName), legalEntityName));
        Parameters.Add(new AuditLogParameter(nameof(applicationNumber), applicationNumber));

        LegalEntityName = legalEntityName;
        ApplicationNumber = applicationNumber;
    }

    public string LegalEntityName { get; }
    public string ApplicationNumber { get; }

    public override string GetDescription()
    {
        return $"Client '{LegalEntityName}' was created from application '{ApplicationNumber}'";
    }
}