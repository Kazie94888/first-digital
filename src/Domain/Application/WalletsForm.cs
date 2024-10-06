using System.Text.Json.Serialization;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Application;
public sealed class WalletsForm : ApplicationForm
{
    internal WalletsForm() { }

    [JsonConstructor]
    internal WalletsForm(List<WalletRecord> wallets)
    {
        Wallets = wallets;
    }

    public List<WalletRecord> Wallets { get; } = [];

    internal Result AddWallet(WalletRecord wallet)
    {
        if (Wallets.Exists(record => record.Address == wallet.Address))
            return Result.Fail(new EntityAlreadyExistsError(nameof(Wallets), $"Wallet address '{wallet.Address}' already exists"));

        Wallets.Add(wallet);
        return Result.Ok();
    }

    internal void ClearWallets() => Wallets.Clear();

    internal override Result Verify(string segmentOrRecord, UserInfo userInfo)
    {
        var walletId = WalletId.Parse(segmentOrRecord);
        if (!Wallets.Exists(x => x.Id == walletId))
            return Result.Fail(new EntityNotFoundError(
                nameof(WalletRecord)));

        Wallets.First(x => x.Id == walletId).Verify(userInfo);

        return Result.Ok();
    }

    internal override bool IsVerified()
    {
        return Wallets.Count > 0
                    && Wallets.TrueForAll(x => x.IsVerified());
    }

    internal override void RequireAdditionalInfo()
    {
        if (!IsVerified())
            return;

        foreach (var item in Wallets)
        {
            item.RemoveVerificationFlag();
        }
    }
}

public sealed record WalletRecord : VerifiableRecord
{
    [JsonConstructor]
    internal WalletRecord(WalletId id, BlockchainNetwork network, string address, DateTimeOffset? createdAt, UserInfo? verifiedBy = null,
                          DateTimeOffset? verifiedAt = null) : base(verifiedBy, verifiedAt)
    {
        Id = id;
        Network = network;
        Address = address;
        CreatedAt = createdAt;
    }

    public WalletId Id { get; }
    public BlockchainNetwork Network { get; }
    public string Address { get; }
    public DateTimeOffset? CreatedAt { get; }
}
