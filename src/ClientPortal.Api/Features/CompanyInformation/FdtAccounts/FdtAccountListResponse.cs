using SmartCoinOS.ClientPortal.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.ClientPortal.Api.Features.CompanyInformation.FdtAccounts;

internal sealed record FdtAccountListResponse
{
    public required FdtAccountId Id { get; init; }
    public string? Alias { get; init; }
    public required FdtAccountDto Account { get; init; }
    public required EntityVerificationStatus Status { get; init; }
    public required bool Archived { get; init; }
}