namespace SmartCoinOS.Domain.Clients;

public sealed record AddressRecord
{
    public AddressRecord() { }

    public required string Country { get; init; }
    public string? State { get; init; }
    public required string PostalCode { get; init; }
    public required string City { get; init; }
    public required string Street { get; init; }
}
