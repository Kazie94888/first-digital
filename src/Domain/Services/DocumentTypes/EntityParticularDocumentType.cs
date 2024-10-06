using SmartCoinOS.Domain.Enums;

namespace SmartCoinOS.Domain.Services.DocumentTypes;

internal sealed record EntityParticularDocumentType() : SmartEnumCollection("EntityParticular",
[
    "Certificate of Incorporation",
    "Memorandum of Association",
    "Articles of Association",
    "Certificate of Incumbency",
    "Register of Directors",
    "Register of Shareholder",
    "Group Ownership And Structure",
    "Company Board Resolution Appointing Additional Users"
]);