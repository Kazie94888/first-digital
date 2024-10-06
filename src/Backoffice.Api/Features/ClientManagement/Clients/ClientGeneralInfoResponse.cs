using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Clients;

public sealed record ClientGeneralInfoResponse
{
    public Guid Id { get; init; }
    public ClientStatus Status { get; init; }
    public required string ClientName { get; init; }
}
