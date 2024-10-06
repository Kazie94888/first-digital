using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Clients.Events;

public sealed class EntityParticularDeclinedEvent : ClientAuditEventBase
{
    public EntityParticularDeclinedEvent(EntityParticularId entityParticularId, string legalEntityName,
        string registrationNumber, ClientId clientId, UserInfo declinedBy)
        : base(clientId, declinedBy)
    {
        LegalEntityName = legalEntityName;
        RegistrationNumber = registrationNumber;

        Parameters.Add(new AuditLogs.AuditLogParameter(nameof(EntityParticularId), entityParticularId.ToString()));
        Parameters.Add(new AuditLogs.AuditLogParameter(nameof(EntityParticular.LegalEntityName), legalEntityName));
        Parameters.Add(new AuditLogs.AuditLogParameter(nameof(EntityParticular.RegistrationNumber), registrationNumber));
    }

    public string LegalEntityName { get; }
    public string RegistrationNumber { get; }

    public override string GetDescription()
    {
        return $"Entity '{LegalEntityName}' (#'{RegistrationNumber}') was declined by {AuditedBy.Username}";
    }
}