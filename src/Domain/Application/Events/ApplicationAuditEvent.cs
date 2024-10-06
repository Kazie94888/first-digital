using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Application.Events;
public abstract class ApplicationAuditEvent : AuditEventBase
{
    protected ApplicationAuditEvent(ApplicationId applicationId, UserInfo auditedBy) : base(auditedBy)
    {
        Parameters.Add(new AuditLogs.AuditLogParameter(nameof(ApplicationId), applicationId.ToString()));
        ApplicationId = applicationId;
    }

    public ApplicationId ApplicationId { get; }
}
