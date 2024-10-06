using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.AuthorizedUsers;

public sealed record AuthorizedUsersResponse
{
    public required AuthorizedUserId Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required AuthorizedUserStatus Status { get; init; }
}