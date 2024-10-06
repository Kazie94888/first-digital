namespace SmartCoinOS.Integrations.FdtPartnerApi.Dto;

public sealed class SupportingDocument
{
    public required string Base64 { get; set; }
    public required string FileName { get; set; }
    public required string ContentType { get; set; }
}
