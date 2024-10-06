namespace SmartCoinOS.Integrations.FdtPartnerApi.Dto;

public sealed class FiatDepositInstructionTransaction
{
    public required int Id { get; init; }
    public required decimal Amount { get; init; }
    public required string AssetSymbol { get; init; }
}