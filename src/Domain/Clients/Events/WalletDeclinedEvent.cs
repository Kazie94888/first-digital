using SmartCoinOS.Domain.AuditLogs;
using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Clients.Events;

public sealed class WalletDeclinedEvent : ClientAuditEventBase
{
    public WalletDeclinedEvent(ClientId clientId, UserInfo declinedBy, string address) : base(clientId, declinedBy)
    {
        Address = address;
        Parameters.Add(new AuditLogParameter(nameof(Address), address));
    }

    public string Address { get; }

    public override string GetDescription()
    {
        return $"Wallet '{Address}' was declined by {AuditedBy.Username}";
    }
}