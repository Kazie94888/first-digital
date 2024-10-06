using SmartCoinOS.Domain.Deposit;
using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Clients.Events;
public class ClientDepositBankChanged : ClientAuditEventBase
{
    public ClientDepositBankChanged(DepositBankId depositBankId, ClientId clientId, UserInfo auditedBy) : base(clientId, auditedBy)
    {
        DepositBankId = depositBankId;

        Parameters.Add(new AuditLogs.AuditLogParameter(nameof(DepositBankId), depositBankId.Value.ToString()));
    }

    public DepositBankId DepositBankId { get; }

    public override string GetDescription()
    {
        return $"Default deposit bank account changed to '{DepositBankId.Value}'";
    }
}
