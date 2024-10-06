namespace SmartCoinOS.Integrations.FdtPartnerApi.Dto;

public sealed class CreateBankAccountResponse
{
    public required int Id { get; set; }
    public required int ClientId { get; set; }
    public string? ClientName { get; set; }
    public string? Name { get; set; }
    public string? AccountHolder { get; set; }
    public string? AccountNumber { get; set; }
    public string? ClearingCode { get; set; }
    public string? BankBranchCode { get; set; }
    public required bool IsPrimary { get; set; }
    public VerificationStatus? VerificationStatus { get; set; }
    public BankDto? Bank { get; set; }
    public string? ClientReference { get; set; }
    public AccountHolderDetail? AccountHolderDetail { get; set; }
}
