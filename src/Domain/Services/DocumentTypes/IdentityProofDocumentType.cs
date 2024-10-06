using SmartCoinOS.Domain.Enums;

namespace SmartCoinOS.Domain.Services.DocumentTypes;

internal sealed record IdentityProofDocumentType() : SmartEnumCollection("IdentityProof",
[
    "Passport",
    "National Identity Card",
    "Social Security Card",
    "Driving Licence",
    "Certificate Of Naturalisation",
    "Work Permit",
    "Residence Permit",
    "Birth Certificate",
    "Marriage Certificate",
    "Citizenship Certificate",
    "Proof Of Identification Of Beneficial Owner",
    "Proof Of Identification Of Director"
]);