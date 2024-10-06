using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Create;

internal sealed record CreateOrderClientsResponse
{
    public ClientId Id { get; init; }
    public required string Name { get; init; }
}
