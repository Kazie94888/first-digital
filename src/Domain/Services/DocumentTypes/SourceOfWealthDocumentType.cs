using SmartCoinOS.Domain.Enums;

namespace SmartCoinOS.Domain.Services.DocumentTypes;

internal sealed record SourceOfWealthDocumentType() : SmartEnumCollection("SourceOfWealth",
[
    "Business Registration",
    "Payslip",
    "Proof Of Source Of Funds"
]);