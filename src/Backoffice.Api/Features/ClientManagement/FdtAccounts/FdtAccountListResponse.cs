using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.FdtAccounts;

public sealed record FdtAccountListResponse
{
    public required FdtAccountId Id { get; init; }
    public required string ClientName { get; init; }
    public required string AccountNumber { get; init; }
    public string? Alias { get; init; }
    public required EntityVerificationStatus Status { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required bool Archived { get; init; }
    public DateTimeOffset? ArchivedAt { get; init; }
}
