using MediatR;
using SmartCoinOS.Domain.AuditLogs;
using SmartCoinOS.Domain.SeedWork;
using SmartCoinOS.Persistence;

namespace SmartCoinOS.Commands.EventHandlers;

internal class AuditEventHandler<TAudit> : INotificationHandler<TAudit> where TAudit : AuditEventBase
{
    private readonly DataContext _dataContext;

    public AuditEventHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public Task Handle(TAudit notification, CancellationToken cancellationToken)
    {
        var parameters = notification.Parameters.ToList();

        var auditLog = AuditLog.Create(
            notification.GetType().Name,
            notification.GetDescription(),
            parameters,
            notification.AuditedBy,
            DateTimeOffset.UtcNow);

        _dataContext.AuditLogs.Add(auditLog);
        return Task.CompletedTask;
    }
}
