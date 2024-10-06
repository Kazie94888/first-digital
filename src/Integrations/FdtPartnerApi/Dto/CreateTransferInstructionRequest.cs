namespace SmartCoinOS.Integrations.FdtPartnerApi.Dto;

public sealed class CreateTransferInstructionRequest
{
    public required int ClientId { get; set; }
    public required int SourceServiceAccountId { get; set; }
    public required int TargetServiceAccountId { get; set; }
    public required decimal Amount { get; set; }
    public required string Memo { get; set; }
}
