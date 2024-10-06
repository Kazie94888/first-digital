namespace SmartCoinOS.Integrations.FdtPartnerApi.Dto;

public sealed class CreateFiatWithdrawResponse
{
    public required int Id { get; set; }
    public DateTimeOffset? DateCreated { get; set; }
    public string? ReferenceNumber { get; set; }
    public required decimal Amount { get; set; }
    public string? Currency { get; set; }
    public PaymentInstructionStatus? Status { get; set; }
    public int? SourceClientId { get; set; }
    public string? SourceClientName { get; set; }
    public string? SourceServiceEntityName { get; set; }
    public int? SourceServiceEntityId { get; set; }
    public string? SourceServiceAccountNumber { get; set; }
    public int? SourceServiceAccountId { get; set; }
    public string? TargetBankAccountBeneficiaryFullName { get; set; }
    public string? TargetBankAccountNumber { get; set; }
    public string? TargetBankName { get; set; }
    public string? TargetBankSwift { get; set; }
}