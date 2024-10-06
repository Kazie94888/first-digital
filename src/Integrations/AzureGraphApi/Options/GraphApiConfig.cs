namespace SmartCoinOS.Integrations.AzureGraphApi.Options;

public sealed class GraphApiConfig
{
    public required string TenantId { get; set; }
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public required string TenantDomainName { get; set; }
}