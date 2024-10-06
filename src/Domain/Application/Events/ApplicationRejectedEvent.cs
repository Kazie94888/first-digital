using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Application.Events;

public sealed class ApplicationRejectedEvent : ApplicationAuditEvent
{
    public ApplicationRejectedEvent(ApplicationId applicationId, string legalEntityName, UserInfo auditedBy) : base(applicationId, auditedBy)
    {
        LegalEntityName = legalEntityName;
    }

    public string LegalEntityName { get; }

    public override string GetDescription()
    {
        return $"Application of '{LegalEntityName}' rejected";
    }
}
