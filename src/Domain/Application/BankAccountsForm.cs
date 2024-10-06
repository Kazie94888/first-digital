using System.Text.Json.Serialization;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.SeedWork;
using SmartCoinOS.Domain.Shared;

namespace SmartCoinOS.Domain.Application;

public sealed class BankAccountsForm : ApplicationForm
{
    internal BankAccountsForm() { }

    [JsonConstructor]
    internal BankAccountsForm(List<BankAccountRecord> bankRecords)
    {
        BankRecords = bankRecords;
    }

    public List<BankAccountRecord> BankRecords { get; } = [];

    internal Result AddBankRecord(BankAccountRecord bankRecord)
    {
        if (BankRecords.Exists(record => record.Iban == bankRecord.Iban))
            return Result.Fail(new EntityAlreadyExistsError(nameof(BankRecords),
                $"Bank account '{bankRecord.Iban}' already exists"));

        BankRecords.Add(bankRecord);

        return Result.Ok();
    }

    internal override Result Verify(string segmentOrRecord, UserInfo userInfo)
    {
        var bankAccountId = BankAccountId.Parse(segmentOrRecord);
        if (BankRecords.TrueForAll(x => x.Id != bankAccountId))
            return Result.Fail(new EntityNotFoundError(nameof(BankAccountRecord)));

        BankRecords.First(x => x.Id == bankAccountId).Verify(userInfo);

        return Result.Ok();
    }

    internal override bool IsVerified()
    {
        return BankRecords.Count > 0 && BankRecords.TrueForAll(x => x.IsVerified());
    }

    internal override void RequireAdditionalInfo()
    {
        if (!IsVerified())
            return;

        foreach (var item in BankRecords)
        {
            item.RemoveVerificationFlag();
        }
    }

    internal bool RemoveDocument(DocumentId documentId)
    {
        var removed = false;
        
        foreach (var documents in BankRecords.Select(r => r.Documents))
        {
            var foundDocument = documents.Find(x => x.DocumentId == documentId);
            if (foundDocument is null)
                continue;

            removed = documents.Remove(foundDocument);
            if (removed)
            {
                RequireAdditionalInfo();
                break;
            }
        }

        return removed;
    }

    internal void Clear() => BankRecords.Clear();
}

public sealed record BankAccountRecord : VerifiableRecord
{
    internal BankAccountRecord(string beneficiary, string bankName, string iban, string swift,
        SmartTrustBank smartTrustBankId, AddressRecord address,
        List<ApplicationDocument> documents)
    {
        Id = BankAccountId.New();
        Beneficiary = beneficiary;
        BankName = bankName;
        Iban = iban;
        Swift = swift;
        SmartTrustOwnBankId = smartTrustBankId.OwnBankId;
        SmartTrustThirdPartyBankId = smartTrustBankId.ThirdPartyBankId;
        Street = address.Street;
        City = address.City;
        PostalCode = address.PostalCode;
        State = address.State;
        Country = address.Country;

        CreatedAt = DateTimeOffset.UtcNow;

        Documents = documents;
    }

    [JsonConstructor]
    internal BankAccountRecord(BankAccountId id, string beneficiary, string bankName, string iban, string swift,
        int? smartTrustOwnBankId, int? smartTrustThirdPartyBankId,
        string street, string city, string postalCode, string? state, string country, DateTimeOffset? createdAt,
        List<ApplicationDocument> documents, UserInfo? verifiedBy = null,
        DateTimeOffset? verifiedAt = null) : base(verifiedBy, verifiedAt)
    {
        Id = id;
        Beneficiary = beneficiary;
        BankName = bankName;
        Iban = iban;
        Swift = swift;
        SmartTrustOwnBankId = smartTrustOwnBankId;
        SmartTrustThirdPartyBankId = smartTrustThirdPartyBankId;
        Street = street;
        City = city;
        PostalCode = postalCode;
        State = state;
        Country = country;
        CreatedAt = createdAt;

        Documents = documents;
    }

    public BankAccountId Id { get; }
    public string Beneficiary { get; }
    public string BankName { get; }
    public string Iban { get; }
    public string Swift { get; }
    public int? SmartTrustOwnBankId { get; }
    public int? SmartTrustThirdPartyBankId { get; }
    public string Street { get; }
    public string City { get; }
    public string PostalCode { get; }
    public string? State { get; }
    public string Country { get; }
    public DateTimeOffset? CreatedAt { get; }
    public List<ApplicationDocument> Documents { get; } = [];
}
