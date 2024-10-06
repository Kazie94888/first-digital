using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Clients.Events;

public sealed class BankAccountArchivedEvent : ClientAuditEventBase
{
    public BankAccountArchivedEvent(
        BankAccountId bankAccountId,
        string bankName,
        string iban,
        ClientId clientId,
        UserInfo archivedBy)
        : base(clientId, archivedBy)
    {
        BankAccountId = bankAccountId;
        BankName = bankName;
        Iban = iban;

        Parameters.Add(new AuditLogs.AuditLogParameter(nameof(BankAccountId), bankAccountId.ToString()));
        Parameters.Add(new AuditLogs.AuditLogParameter(nameof(BankName), bankName));
        Parameters.Add(new AuditLogs.AuditLogParameter(nameof(Iban), iban));
    }

    public BankAccountId BankAccountId { get; }
    public string BankName { get; }
    public string Iban { get; }

    public override string GetDescription()
    {
        return $"Account '{Iban}' (#'{BankName}') archived";
    }
}