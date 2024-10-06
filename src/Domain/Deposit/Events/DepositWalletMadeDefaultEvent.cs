using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Deposit.Events;

public sealed class DepositWalletMadeDefaultEvent : DepositWalletAuditEventBase
{
    public DepositWalletMadeDefaultEvent(DepositWalletId id, WalletAccount account, UserInfo userInfo) : base(id, userInfo)
    {
        Parameters.Add(new AuditLogs.AuditLogParameter(nameof(account.Address), account.Address));
        Parameters.Add(new AuditLogs.AuditLogParameter(nameof(account.Network), account.Network.ToString()));

        Address = account.Address;
        Network = account.Network;
        Id = id;
    }
    public string Address { get; }
    public BlockchainNetwork Network { get; }
    public DepositWalletId Id { get; }

    public override string GetDescription()
    {
        return $"Deposit wallet {Network}: '{Address}' was set as default";
    }
}