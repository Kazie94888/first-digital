using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.BankAccounts;

public sealed record BankAccountListResponse
{
    public required BankAccountId Id { get; init; }
    public required string Beneficiary { get; init; }
    public required string Iban { get; init; }
    public string? Alias { get; init; }
    public required EntityVerificationStatus Status { get; init; }
    public required string BankName { get; init; }
    public required string SwiftCode { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required bool Archived { get; init; }
    public DateTimeOffset? ArchivedAt { get; init; }
}