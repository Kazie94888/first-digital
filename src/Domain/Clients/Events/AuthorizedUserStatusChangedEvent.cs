using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Clients.Events;

public sealed class AuthorizedUserStatusChangedEvent : ClientAuditEventBase
{
    public AuthorizedUserStatusChangedEvent(
        string email,
        AuthorizedUserStatus status,
        ClientId clientId,
        UserInfo changedBy)
        : base(clientId, changedBy)
    {
        Email = email;
        Status = status;


        Parameters.Add(new AuditLogs.AuditLogParameter(nameof(Email), email));
        Parameters.Add(new AuditLogs.AuditLogParameter(nameof(Status), status.ToString()));
    }

    public string Email { get; }
    public AuthorizedUserStatus Status { get; }

    public override string GetDescription()
    {
        return $"User '{Email}' status changed to {Status}";
    }
}