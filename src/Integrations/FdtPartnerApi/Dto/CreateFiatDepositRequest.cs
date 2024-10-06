namespace SmartCoinOS.Integrations.FdtPartnerApi.Dto;

public sealed class CreateFiatDepositRequest
{
    public required int ClientId { get; set; }
    public required int TargetServiceAccountId { get; set; }
    public required int SourceBankAccountId { get; set; }
    public required decimal Amount { get; set; }
    public required SourceOfFunds SourceOfFunds { get; set; }
}
