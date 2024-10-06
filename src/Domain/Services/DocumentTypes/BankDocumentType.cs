using SmartCoinOS.Domain.Enums;

namespace SmartCoinOS.Domain.Services.DocumentTypes;

internal sealed record BankDocumentType() : SmartEnumCollection("BankDocument",
[
    "Bank Statement",
    "Credit Card Statement",
    "Deposit Slip",
    "Withdrawal Completion Slip",
    "Payment Completion Slip"
]);