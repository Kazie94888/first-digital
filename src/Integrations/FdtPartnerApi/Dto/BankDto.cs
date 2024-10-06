namespace SmartCoinOS.Integrations.FdtPartnerApi.Dto;

public sealed class BankDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public string? Branch { get; set; }
    public required string SwiftCode { get; set; }
    public required int CountryId { get; set; }
    public string? City { get; set; }
    public required bool IsDeleted { get; set; }
}