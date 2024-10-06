using SmartCoinOS.Domain.AuditLogs;
using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Clients.Events;

public sealed class WalletArchivedEvent : ClientAuditEventBase
{
    public WalletArchivedEvent(WalletId walletId, WalletAccount walletAccount, ClientId clientId, UserInfo archivedBy)
        : base(clientId, archivedBy)
    {
        WalletId = walletId;
        Account = walletAccount;

        Parameters.Add(new AuditLogParameter(nameof(Account.Address), walletAccount.Address));
        Parameters.Add(new AuditLogParameter(nameof(Account.Network), walletAccount.Network.ToString()));
    }

    public WalletId WalletId { get; }
    public WalletAccount Account { get; }

    public override string GetDescription()
    {
        return $"Wallet {Account.Address} ({Account.Network}) archived";
    }
}