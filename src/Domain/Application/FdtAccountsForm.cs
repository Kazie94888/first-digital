using System.Text.Json.Serialization;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Application;
public sealed class FdtAccountsForm : ApplicationForm
{
    internal FdtAccountsForm() { }

    [JsonConstructor]
    internal FdtAccountsForm(List<FdtAccountRecord> fdtAccounts)
    {
        FdtAccounts = fdtAccounts;
    }

    public List<FdtAccountRecord> FdtAccounts { get; } = [];

    internal Result AddAccount(FdtAccountRecord account)
    {
        if (FdtAccounts.Exists(record => record.AccountNumber == account.AccountNumber))
            return Result.Fail(new EntityAlreadyExistsError(nameof(FdtAccounts), $"FDT account '{account.AccountNumber}' already exists"));

        FdtAccounts.Add(account);
        return Result.Ok();
    }

    internal override Result Verify(string segmentOrRecord, UserInfo userInfo)
    {
        var fdtAccountId = FdtAccountId.Parse(segmentOrRecord);
        if (!FdtAccounts.Exists(x => x.Id == fdtAccountId))
            return Result.Fail(new EntityNotFoundError(nameof(FdtAccountRecord)));

        FdtAccounts.First(x => x.Id == fdtAccountId).Verify(userInfo);

        return Result.Ok();
    }

    internal void ClearAccounts() => FdtAccounts.Clear();

    internal override bool IsVerified()
    {
        return FdtAccounts.Count > 0
                    && FdtAccounts.TrueForAll(x => x.IsVerified());
    }

    internal override void RequireAdditionalInfo()
    {
        if (!IsVerified())
            return;

        foreach (var item in FdtAccounts)
        {
            item.RemoveVerificationFlag();
        }
    }
}

public sealed record FdtAccountRecord : VerifiableRecord
{
    [JsonConstructor]
    internal FdtAccountRecord(FdtAccountId id, string clientName, string accountNumber, DateTimeOffset? createdAt, UserInfo? verifiedBy = null,
                              DateTimeOffset? verifiedAt = null) : base(verifiedBy, verifiedAt)
    {
        Id = id;
        ClientName = clientName;
        AccountNumber = accountNumber;
        CreatedAt = createdAt;
    }

    public FdtAccountId Id { get; }
    public string ClientName { get; }
    public string AccountNumber { get; }
    public DateTimeOffset? CreatedAt { get; }
}
