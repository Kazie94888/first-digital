namespace SmartCoinOS.Integrations.FdtPartnerApi.Dto;

public sealed class AccountHolderDetail
{
    public required string AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public required string City { get; set; }
    public string? PostalCode { get; set; }
    public string? StateProvince { get; set; }
    public required int CountryId { get; set; }
    public required string Relationship { get; set; }
}