using SmartCoinOS.Domain.Application;

namespace SmartCoinOS.ClientPortal.Api.Features.CompanyInformation.EntityParticulars;

internal sealed record EntityParticularsResponse
{
    public required Address Address { get; init; }
    public required Particulars Particulars { get; init; }
    public required Contacts Contacts { get; init; }
}

internal sealed record Particulars
{
    public required CompanyLegalStructure LegalStructure { get; init; }
    public string? LegalStructureOther { get; init; }
    public required string RegistrationNumber { get; init; }
    public required DateOnly DateOfInc { get; init; }
    public required string CountryOfInc { get; init; }
}

internal sealed record Address
{
    public required string FullAddress { get; init; }
}

internal sealed record Contacts
{
    public required string Email { get; init; }
    public required string Phone { get; init; }
}