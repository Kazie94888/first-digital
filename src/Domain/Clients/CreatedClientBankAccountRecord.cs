namespace SmartCoinOS.Domain.Clients;

public sealed record CreatedClientBankAccountRecord
{
    public required string Beneficiary { get; init; }
    public required string Iban { get; init; }
    public required string BankName { get; init; }
    public required string SwiftCode { get; init; }
    public string? Alias { get; init; }
    public required SmartTrustBank SmartTrustBank { get; init; }
    public required AddressRecord Address { get; init; }
}
