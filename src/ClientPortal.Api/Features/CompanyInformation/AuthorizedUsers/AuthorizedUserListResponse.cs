using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.ClientPortal.Api.Features.CompanyInformation.AuthorizedUsers;

internal sealed record AuthorizedUserListResponse
{
    public required AuthorizedUserId Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required string ExternalId { get; init; }
    public required AuthorizedUserStatus Status { get; init; }
}
