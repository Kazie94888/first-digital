using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.FdtAccounts;

public sealed record FdtAccountDetailsResponse
{
    public required FdtAccountId Id { get; init; }
    public required string AccountNumber { get; init; }
    public required string ClientName { get; init; }
    public string? Alias { get; init; }
    public required EntityVerificationStatus Status { get; init; }
    public required bool Archived { get; init; }
    public DateTimeOffset? ArchivedAt { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required UserInfoDto CreatedBy { get; init; }
}