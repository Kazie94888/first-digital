namespace SmartCoinOS.Integrations.FdtPartnerApi.Dto;

public sealed class CreateTransferInstructionResponse
{
    public required int Id { get; set; }
    public DateTimeOffset DateCreated { get; set; }
    public required string ReferenceNumber { get; set; }
    public required decimal Amount { get; set; }
    public required string Currency { get; set; }
    public PaymentInstructionStatus? Status { get; set; }
    public int? SourceClientId { get; set; }
    public string? SourceClientName { get; set; }
    public int? SourceServiceEntityId { get; set; }
    public string? SourceServiceEntityName { get; set; }
    public int? SourceServiceAccountId { get; set; }
    public string? SourceServiceAccountNumber { get; set; }
    public string? TargetClientName { get; set; }
    public required string TargetServiceAccountNumber { get; set; }
    public required string Memo { get; set; }
}