using System.Text.Json.Serialization;
using SmartCoinOS.Domain.Application.Enums;
using SmartCoinOS.Domain.SeedWork;
using SmartCoinOS.Domain.Shared;

namespace SmartCoinOS.Domain.Application;

public sealed class EntityParticularsForm : ApplicationForm
{
    internal EntityParticularsForm() { }

    public EntityParticularsForm(
        CompanyParticulars particulars,
        RegistrationAddress registrationAddress,
        CompanyContact contact)
    {
        Particulars = particulars;
        RegistrationAddress = registrationAddress;
        Contact = contact;
    }

    public required CompanyParticulars Particulars { get; init; }
    public required RegistrationAddress RegistrationAddress { get; init; }
    public required CompanyContact Contact { get; init; }


    internal override bool IsVerified()
    {
        return Particulars.IsVerified()
               && RegistrationAddress.IsVerified()
               && Contact.IsVerified();
    }

    internal override void RequireAdditionalInfo()
    {
        if (!IsVerified())
            return;

        Particulars.RemoveVerificationFlag();
        RegistrationAddress.RemoveVerificationFlag();
        Contact.RemoveVerificationFlag();
    }

    internal bool RemoveDocument(DocumentId documentId)
    {
        var foundDocument = Particulars.Documents.Find(x => x.DocumentId == documentId);
        if (foundDocument is null)
            return false;

        var removed = Particulars.Documents.Remove(foundDocument);
        if (removed)
            RequireAdditionalInfo();

        return removed;
    }

    internal override Result Verify(string segmentOrRecord, UserInfo userInfo)
    {
        return segmentOrRecord switch
        {
            EntityParticularRecords.CompanyParticulars => Particulars.Verify(userInfo),
            EntityParticularRecords.RegistrationAddress => RegistrationAddress.Verify(userInfo),
            EntityParticularRecords.ContactForm => Contact.Verify(userInfo),

            _ => throw new InvalidOperationException()
        };
    }
}

public sealed record CompanyParticulars : VerifiableRecord
{
    internal CompanyParticulars() { }

    [JsonConstructor]
    internal CompanyParticulars(
        CompanyLegalStructure legalStructure,
        string? structureDetails,
        string registrationNumber,
        DateOnly dateOfInc,
        string countryOfInc,
        List<ApplicationDocument> documents,
        UserInfo? verifiedBy = null,
        DateTimeOffset? verifiedAt = null) : base(verifiedBy, verifiedAt)
    {
        LegalStructure = legalStructure;
        StructureDetails = structureDetails;
        RegistrationNumber = registrationNumber;
        DateOfInc = dateOfInc;
        CountryOfInc = countryOfInc;
        Documents = documents;
    }

    public required CompanyLegalStructure LegalStructure { get; init; }
    public string? StructureDetails { get; init; }
    public required string RegistrationNumber { get; init; }
    public required DateOnly DateOfInc { get; init; }
    public required string CountryOfInc { get; init; }
    public List<ApplicationDocument> Documents { get; set; } = [];
}

public sealed record RegistrationAddress : VerifiableRecord
{
    internal RegistrationAddress() { }

    [JsonConstructor]
    internal RegistrationAddress(
        string street,
        string postalCode,
        string? state,
        string country,
        string city,
        UserInfo? verifiedBy = null,
        DateTimeOffset? verifiedAt = null) : base(verifiedBy, verifiedAt)
    {
        Street = street;
        PostalCode = postalCode;
        State = state;
        Country = country;
        City = city;
    }

    public required string Street { get; init; }
    public required string PostalCode { get; init; }
    public string? State { get; init; }
    public required string Country { get; init; }
    public required string City { get; init; }
}

public sealed record CompanyContact : VerifiableRecord
{
    internal CompanyContact() { }

    [JsonConstructor]
    internal CompanyContact(string email,
                            string phone,
                            UserInfo? verifiedBy = null,
                            DateTimeOffset? verifiedAt = null) : base(verifiedBy, verifiedAt)
    {
        Email = email;
        Phone = phone;
    }

    public required string Email { get; init; }
    public required string Phone { get; init; }
}
