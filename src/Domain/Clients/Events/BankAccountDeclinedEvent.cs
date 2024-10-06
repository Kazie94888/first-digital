using SmartCoinOS.Domain.AuditLogs;
using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Clients.Events;

public sealed class BankAccountDeclinedEvent : ClientAuditEventBase
{
    public BankAccountDeclinedEvent(ClientId clientId, UserInfo declinedBy, string iban) : base(clientId, declinedBy)
    {
        Iban = iban;
        Parameters.Add(new AuditLogParameter(nameof(Iban), iban));
    }

    public string Iban { get; }

    public override string GetDescription()
    {
        return $"Bank Account '{Iban}' was declined by {AuditedBy.Username}";
    }
}