namespace SmartCoinOS.Integrations.FdtPartnerApi.Services;
public sealed class FdtPartnerSettings
{
    public required string ApiUrl { get; set; }
    public required short ApiTimeout { get; set; }
    public required string ApiClientId { get; set; }
    public required string ApiClientSecret { get; set; }
    public required string ApiScope { get; set; }
    public required int FdtClientId { get; set; }
    public required int ServiceAccountId { get; set; }
}