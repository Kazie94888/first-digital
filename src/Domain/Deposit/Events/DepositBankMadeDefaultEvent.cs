using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Deposit.Events;

public sealed class DepositBankMadeDefaultEvent : DepositBankAuditEventBase
{
    public DepositBankMadeDefaultEvent(DepositBankId id, string name, string beneficiary, string iban, UserInfo auditedBy) : base(id, auditedBy)
    {
        Parameters.Add(new AuditLogs.AuditLogParameter(nameof(name), name));
        Parameters.Add(new AuditLogs.AuditLogParameter(nameof(beneficiary), beneficiary));
        Parameters.Add(new AuditLogs.AuditLogParameter(nameof(iban), iban));
        Id = id;
        Name = name;
        Beneficiary = beneficiary;
        Iban = iban;
    }

    public DepositBankId Id { get; }
    public string Name { get; }
    public string Beneficiary { get; }
    public string Iban { get; }

    public override string GetDescription()
    {
        return $"Deposit bank '{Name} ({Iban})' was set as default";
    }
}
