namespace SmartCoinOS.Integrations.FdtPartnerApi.Dto;

public sealed class CreateFiatPaymentInstructionRequest
{
    public required int ClientId { get; set; }
    public required int SourceServiceAccountId { get; set; }
    public required int TargetBankAccountId { get; set; }
    public required decimal Amount { get; set; }
    public PurposeOfPayment? PurposeOfPayment { get; set; }
    public SupportingDocument? SupportingDocuments { get; set; }
    public string? PaymentReference { get; set; }
    public string? Remarks { get; set; }
}
