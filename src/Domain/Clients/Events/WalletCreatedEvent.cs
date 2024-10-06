using SmartCoinOS.Domain.AuditLogs;
using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Clients.Events;

public class WalletCreatedEvent : ClientAuditEventBase
{
    public WalletCreatedEvent(WalletId walletId, WalletAccount walletAccount, ClientId clientId, UserInfo auditedBy) : base(clientId, auditedBy)
    {
        WalletId = walletId;
        WalletAccount = walletAccount;

        Parameters.Add(new AuditLogParameter(nameof(WalletId), walletId.ToString()));
        Parameters.Add(new AuditLogParameter(nameof(WalletAccount.Address), walletAccount.Address));
        Parameters.Add(new AuditLogParameter(nameof(WalletAccount.Network), walletAccount.Network.ToString()));
    }

    public WalletId WalletId { get; }
    public WalletAccount WalletAccount { get; }

    public override string GetDescription()
    {
        return $"Wallet '{WalletAccount.Address}' at '{WalletAccount.Network}' created by '{AuditedBy.Username}'";
    }
}
