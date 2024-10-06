using SmartCoinOS.Domain.Enums;

namespace SmartCoinOS.Domain.Services.DocumentTypes;

internal sealed record DueDiligenceDocumentType() : SmartEnumCollection("DueDiligence",
[
    "Bank Statement",
    "Register of Directors",
    "Certificate of Incumbency",
    "Certificate of Insurance",
    "Articles of Association",
    "Register of Shareholder",
    "Group Ownership And Structure",
    "Certificate of Incorporation",
    "Company Board Resolution Appointing Additional Users",
    "Government Letter",
    "Home Office Letter",
    "Service Agreement",
    "Immigration Status Document",
    "Tax Identification Document",
    "Tax Receipt",
    "W8Ben Document",
    "Loss Us Nationality"
]);