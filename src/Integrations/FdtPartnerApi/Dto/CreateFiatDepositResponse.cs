namespace SmartCoinOS.Integrations.FdtPartnerApi.Dto;

public sealed class CreateFiatDepositResponse
{
    public required int Id { get; set; }
    public DateTimeOffset? DateCreated { get; set; }
    public required string ReferenceNumber { get; set; }
    public required decimal Amount { get; set; }
    public string? Currency { get; set; }
    public PaymentInstructionStatus? Status { get; set; }
    public int? TargetClientId { get; set; }
    public string? TargetClientName { get; set; }
    public string? TargetBankAccountBeneficiaryName { get; set; }
    public string? TargetBankAccountNumber { get; set; }
    public string? TargetBankName { get; set; }
    public string? TargetBankSwift { get; set; }
    public int? TargetServiceEntityId { get; set; }
    public string? TargetServiceEntityName { get; set; }
    public int? TargetServiceAccountId { get; set; }
    public string? TargetServiceAccountNumber { get; set; }
    public int? SourceBankAccountId { get; set; }
    public string? SourceBankAccountNumber { get; set; }
    public string? SourceBankAccountOwner { get; set; }
    public string? SourceBankName { get; set; }
    public string? SourceBankSwift { get; set; }
    public SourceOfFunds? SourceOfFunds { get; set; }
    public SupportingDocument? SupportingDocument { get; set; }
    public string? ClientReference { get; set; }
    public string? Remark { get; set; }
    public string? TargetBankCity { get; set; }
    public string? TargetBankLine1 { get; set; }
    public string? TargetBankLine2 { get; set; }
    public string? TargetBankPostalCode { get; set; }
    public string? TargetBankStateProvince { get; set; }
    public string? TargetBankCountry { get; set; }
    public string? TargetBankBeneficiaryCity { get; set; }
    public string? TargetBankBeneficiaryLine1 { get; set; }
    public string? TargetBankBeneficiaryLine2 { get; set; }
    public string? TargetBankBeneficiaryPostalCode { get; set; }
    public string? TargetBankBeneficiaryStateProvince { get; set; }
    public string? TargetBankBeneficiaryCountry { get; set; }
}
