using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Clients.Events;

public sealed class AuthorizedUserCreatedEvent : ClientAuditEventBase
{
    public AuthorizedUserCreatedEvent(
        string firstName,
        string lastName,
        string email,
        ClientId clientId,
        UserInfo createdBy)
        : base(clientId, createdBy)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;

        Parameters.Add(new AuditLogs.AuditLogParameter(nameof(FirstName), firstName));
        Parameters.Add(new AuditLogs.AuditLogParameter(nameof(LastName), lastName));
        Parameters.Add(new AuditLogs.AuditLogParameter(nameof(Email), email));
    }

    public string FirstName { get; }
    public string LastName { get; }
    public string Email { get; }

    public override string GetDescription()
    {
        return $"User '{FirstName} {LastName}' with email {Email} created";
    }
}