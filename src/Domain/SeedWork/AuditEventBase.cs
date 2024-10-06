using MediatR;
using SmartCoinOS.Domain.AuditLogs;

namespace SmartCoinOS.Domain.SeedWork;

public abstract class AuditEventBase : INotification
{
    protected AuditEventBase(UserInfo auditedBy)
    {
        Parameters = [];
        AuditedBy = auditedBy;
        Timestamp = DateTimeOffset.UtcNow;
    }

    public DateTimeOffset Timestamp { get; }
    public List<AuditLogParameter> Parameters { get; }
    public UserInfo AuditedBy { get; }

    public abstract string GetDescription();
}