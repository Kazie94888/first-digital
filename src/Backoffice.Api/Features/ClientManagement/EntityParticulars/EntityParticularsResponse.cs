using SmartCoinOS.Domain.Application;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.EntityParticulars;

public sealed record EntityParticularsResponse
{
    public required Address Address { get; init; }
    public required Particulars Particulars { get; init; }
    public required Contacts Contacts { get; init; }
}

public sealed record Particulars
{
    public required CompanyLegalStructure LegalStructure { get; init; }
    public string? LegalStructureOther { get; init; }
    public required string RegistrationNumber { get; init; }
    public required DateOnly DateOfInc { get; init; }
    public required string CountryOfInc { get; init; }
}

public sealed record Address
{
    public required string FullAddress { get; init; }
}

public sealed record Contacts
{
    public required string Email { get; init; }
    public required string Phone { get; init; }
}
