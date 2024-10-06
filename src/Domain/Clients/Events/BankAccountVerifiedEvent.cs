using SmartCoinOS.Domain.AuditLogs;
using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Clients.Events;

public sealed class BankAccountVerifiedEvent : ClientAuditEventBase
{
    public BankAccountVerifiedEvent(ClientId clientId, UserInfo verifiedBy, string iban) : base(clientId, verifiedBy)
    {
        Iban = iban;
        Parameters.Add(new AuditLogParameter(nameof(Iban), iban));
    }

    public string Iban { get; }

    public override string GetDescription()
    {
        return $"Bank Account '{Iban}' was verified by {AuditedBy.Username}";
    }
}