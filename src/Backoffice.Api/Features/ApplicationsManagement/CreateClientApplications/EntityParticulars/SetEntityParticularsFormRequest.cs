using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Application;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.EntityParticulars;

public sealed record SetEntityParticularsFormRequest
{
    public required CompanyParticular CompanyParticulars { get; init; }
    public required RegistrationAddress RegistrationAddress { get; init; }
    public required CompanyContact CompanyContact { get; init; }
}

public sealed record CompanyParticular
{
    public required CompanyLegalStructure LegalStructure { get; init; }
    public string? StructureDetails { get; init; }
    public required string RegistrationNumber { get; init; }
    public required DateOnly DateOfInc { get; init; }
    public required string CountryOfInc { get; init; }

    public required List<DocumentDto> Documents { get; init; } = [];
}

public sealed record RegistrationAddress
{
    public required string Street { get; init; }
    public required string PostalCode { get; init; }
    public required string City { get; init; }
    public string? State { get; init; }
    public required string Country { get; init; }
}

public sealed record CompanyContact
{
    public required string Email { get; init; }
    public required string Phone { get; init; }
}
