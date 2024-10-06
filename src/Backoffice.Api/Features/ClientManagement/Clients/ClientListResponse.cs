using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Clients;

public sealed record ClientListResponse
{
    public ClientId Id { get; init; }
    public required string ClientName { get; init; }
    public required ClientStatus Status { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public required ClientListOrders Orders { get; init; }
    public required MoneyDto Minted { get; init; }
    public required MoneyDto Redeemed { get; init; }
}

public sealed record ClientListOrders
{
    public int Active { get; init; }
    public int Total { get; init; }
}