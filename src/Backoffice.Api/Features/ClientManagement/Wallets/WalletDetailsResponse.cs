using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Wallets;

public sealed record WalletDetailsResponse
{
    public required WalletId Id { get; init; }
    public required WalletAccount Address { get; init; }
    public string? Alias { get; init; }
    public required EntityVerificationStatus Status { get; init; }
    public required bool Archived { get; init; }
    public DateTimeOffset? ArchivedAt { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required string CreatedBy { get; init; }
}