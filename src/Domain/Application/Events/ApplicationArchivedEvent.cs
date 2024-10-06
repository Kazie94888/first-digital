using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Application.Events;
public sealed class ApplicationArchivedEvent : ApplicationAuditEvent
{
    public ApplicationArchivedEvent(ApplicationId applicationId, UserInfo auditedBy) : base(applicationId, auditedBy)
    {
    }

    public override string GetDescription()
    {
        return "Application archived";
    }
}
