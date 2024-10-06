using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Application.Events;

public sealed class ApplicationApprovedAuditEvent : ApplicationAuditEvent
{
    public ApplicationApprovedAuditEvent(ApplicationId applicationId, string legalEntityName, UserInfo auditedBy) : base(applicationId, auditedBy)
    {
        LegalEntityName = legalEntityName;
    }

    public string LegalEntityName { get; }

    public override string GetDescription()
    {
        return $"Application of '{LegalEntityName}' approved";
    }
}
