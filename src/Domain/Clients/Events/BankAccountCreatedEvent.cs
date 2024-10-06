using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Clients.Events;

public sealed class BankAccountCreatedEvent : ClientAuditEventBase
{
    public BankAccountCreatedEvent(
        BankAccountId bankAccountId,
        string bankName,
        string beneficiary,
        string iban,
        string swiftCode,
        ClientId clientId,
        UserInfo createdBy)
        : base(clientId, createdBy)
    {
        BankAccountId = bankAccountId;
        BankName = bankName;
        Beneficiary = beneficiary;
        Iban = iban;
        SwiftCode = swiftCode;

        Parameters.Add(new AuditLogs.AuditLogParameter(nameof(BankAccountId), bankAccountId.ToString()));
        Parameters.Add(new AuditLogs.AuditLogParameter(nameof(BankName), bankName));
        Parameters.Add(new AuditLogs.AuditLogParameter(nameof(Beneficiary), beneficiary));
        Parameters.Add(new AuditLogs.AuditLogParameter(nameof(Iban), iban));
        Parameters.Add(new AuditLogs.AuditLogParameter(nameof(SwiftCode), swiftCode));
    }

    public BankAccountId BankAccountId { get; }
    public string BankName { get; }
    public string Beneficiary { get; }
    public string Iban { get; }
    public string SwiftCode { get; }

    public override string GetDescription()
    {
        return $"Account '{Iban}' (#'{BankName}') created for {Beneficiary}";
    }
}