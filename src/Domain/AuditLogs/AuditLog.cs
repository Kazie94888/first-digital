using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.AuditLogs;

public sealed class AuditLog : AggregateRoot
{
    private AuditLog() { }

    public required AuditLogId Id { get; init; }
    public required string Event { get; init; }
    public required string EventDescription { get; init; }
    public required DateTimeOffset Timestamp { get; init; }

    private readonly List<AuditLogParameter> _parameters = [];
    public IReadOnlyList<AuditLogParameter> Parameters => _parameters;

    private void AddParameters(List<AuditLogParameter> parameters)
    {
        _parameters.AddRange(parameters);
    }

    public static AuditLog Create(string action, string description, List<AuditLogParameter> parameters, UserInfo createdBy, DateTimeOffset timestamp)
    {
        var auditLog = new AuditLog
        {
            Id = AuditLogId.New(),
            Event = action,
            EventDescription = description,
            Timestamp = timestamp,
            CreatedBy = createdBy
        };

        auditLog.AddParameters(parameters);

        return auditLog;
    }

    public string? GetParameter(string name)
    {
        return _parameters.Where(param => param.Name == name)
                                                    .Select(param => param.Value)
                                                    .FirstOrDefault();
    }
}
