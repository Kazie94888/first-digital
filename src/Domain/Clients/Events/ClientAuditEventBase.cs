using SmartCoinOS.Domain.AuditLogs;
using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Clients.Events;

public abstract class ClientAuditEventBase : AuditEventBase
{
    protected ClientAuditEventBase(ClientId clientId, UserInfo auditedBy) : base(auditedBy)
    {
        Parameters.Add(new AuditLogParameter(AuditParameters.ClientId, clientId.Value.ToString()));
    }
}