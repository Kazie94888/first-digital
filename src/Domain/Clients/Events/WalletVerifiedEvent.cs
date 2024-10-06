using SmartCoinOS.Domain.AuditLogs;
using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Clients.Events;

public sealed class WalletVerifiedEvent : ClientAuditEventBase
{
    public WalletVerifiedEvent(ClientId clientId, UserInfo verifiedBy, string address) : base(clientId, verifiedBy)
    {
        Address = address;
        Parameters.Add(new AuditLogParameter(nameof(Address), address));
    }

    public string Address { get; }

    public override string GetDescription()
    {
        return $"Wallet '{Address}' was verified by {AuditedBy.Username}";
    }
}