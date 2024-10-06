using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.ClientPortal.Api.Base.Dto;

public sealed record BlockchainAddressDto
{
    public required string Address { get; init; }
    public BlockchainNetwork Network { get; init; }
}