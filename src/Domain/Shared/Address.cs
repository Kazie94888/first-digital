using System.Text;
using SmartCoinOS.Domain.SeedWork;
using StronglyTypedIds;

namespace SmartCoinOS.Domain.Shared;

public sealed class Address : Entity
{
    internal Address() { }

    public required AddressId Id { get; init; }
    public required string Country { get; init; }
    public string? State { get; init; }
    public required string PostalCode { get; init; }
    public required string City { get; init; }
    public required string Street { get; init; }

    public bool Archived { get; private set; }
    public DateTimeOffset? ArchivedAt { get; private set; }

    public void Archive()
    {
        Archived = true;
        ArchivedAt = DateTimeOffset.UtcNow;
    }

    public string GetFullAddress()
    {
        var fullAddress = new StringBuilder();

        fullAddress.Append($"{Street}, {City} {PostalCode}");

        if (!string.IsNullOrEmpty(State))
            fullAddress.Append($", {State}");

        fullAddress.Append($", {Country}");

        return fullAddress.ToString();
    }
}

[StronglyTypedId]
public readonly partial struct AddressId;