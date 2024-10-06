using SmartCoinOS.Domain.Application;

namespace SmartCoinOS.Domain.Clients;

public sealed class EntityParticular : ClientAsset
{
    internal EntityParticular()
    {
    }

    public required EntityParticularId Id { get; init; }
    public required string LegalEntityName { get; init; }
    public required string RegistrationNumber { get; init; }
    public required CompanyLegalStructure LegalStructure { get; init; }
    public string? StructureDetails { get; init; }
    public required DateOnly DateOfIncorporation { get; init; }
    public required string CountryOfInc { get; init; }

    public ClientId ClientId { get; init; }

    public bool IsVerified()
    {
        return VerificationStatus == EntityVerificationStatus.Verified && !Archived;
    }
}