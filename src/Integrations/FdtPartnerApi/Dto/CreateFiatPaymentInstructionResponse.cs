namespace SmartCoinOS.Integrations.FdtPartnerApi.Dto;

public sealed class CreateFiatPaymentInstructionResponse
{
    public required int Id { get; init; }
    public required string ReferenceNumber { get; init; }
}
