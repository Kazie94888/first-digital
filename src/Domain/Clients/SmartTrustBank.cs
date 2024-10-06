namespace SmartCoinOS.Domain.Clients;

public sealed record SmartTrustBank
{
    public SmartTrustBank(int? ownBankId, int? thirdPartyBankId)
    {
        OwnBankId = ownBankId;
        ThirdPartyBankId = thirdPartyBankId;
    }

    public int? OwnBankId { get; init; }
    public int? ThirdPartyBankId { get; init; }
}