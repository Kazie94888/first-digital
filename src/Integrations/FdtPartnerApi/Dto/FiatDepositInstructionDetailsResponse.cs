namespace SmartCoinOS.Integrations.FdtPartnerApi.Dto;

public sealed class FiatDepositInstructionDetailsResponse
{
    public required int Id { get; set; }
    public required decimal Amount { get; set; }
    public required string Currency { get; set; }
}
