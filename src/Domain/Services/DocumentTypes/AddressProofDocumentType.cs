using SmartCoinOS.Domain.Enums;

namespace SmartCoinOS.Domain.Services.DocumentTypes;

internal sealed record AddressProofDocumentType() : SmartEnumCollection("AddressProof",
[
    "Utility Bill - Electric",
    "Utility Bill - Gas",
    "Utility Bill - Other",
    "Utility Bill - Water",
    "Utility Bill - Landline Phone",
    "Utility Bill - Internet",
    "Utility Bill - Mobile",
    "Credit Card Statement",
    "Proof Of Address Of Director",
    "Proof Of Address Of Beneficial Owner"
]);