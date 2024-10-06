namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.AuthorizedUsers;

public sealed record AuthorizedUsersCountResponse
{
    public required int Active { get; init; }
    public required int Archived { get; init; }
}