using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Wallets;

public sealed record WalletsResponse
{
    public WalletId Id { get; init; }
    public string? Address { get; init; }
    public BlockchainNetwork Network { get; init; }
    public string? Alias { get; init; }
    public EntityVerificationStatus Status { get; init; }
    public int OrderCount { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public bool Archived { get; init; }
    public DateTimeOffset? ArchivedAt { get; init; }
}
