namespace SmartCoinOS.Integrations.FdtPartnerApi.Dto;

public sealed class CreateFiatWithdrawRequest
{
    public required int ClientId { get; set; }
    public required int SourceServiceAccountId { get; set; }
    public required int BeneficiaryBankAccountId { get; set; }
    public required decimal Amount { get; set; }
}
