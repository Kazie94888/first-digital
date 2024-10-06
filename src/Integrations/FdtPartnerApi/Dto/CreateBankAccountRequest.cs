namespace SmartCoinOS.Integrations.FdtPartnerApi.Dto;

public sealed class CreateBankAccountRequest
{
    public required int ClientId { get; set; }
    public required int BankId { get; set; }
    public required string AccountHolder { get; set; }
    public required string AccountNumber { get; set; }
    public string? ClearingCode { get; set; }
    public string? ClientReference { get; set; }
    public required AccountHolderDetail AccountHolderDetail { get; set; }
    public int? IntermediaryBankId { get; set; }
}
