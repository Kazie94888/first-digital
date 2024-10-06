using SmartCoinOS.ClientPortal.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.ClientPortal.Api.Features.CompanyInformation.Wallets;

internal sealed record WalletListResponse
{
    public required WalletId Id { get; init; }
    public string? Alias { get; init; }
    public required BlockchainAddressDto Address { get; init; }
    public required EntityVerificationStatus Status { get; init; }
    public required bool Archived { get; init; }
    public required int OrderCount { get; init; }
}
