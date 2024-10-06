using SmartCoinOS.Domain.SeedWork;
using StronglyTypedIds;

namespace SmartCoinOS.Domain.Clients;

public sealed class AuthorizedUser : Entity
{
    internal AuthorizedUser()
    {
        Status = AuthorizedUserStatus.Active;
    }

    public required AuthorizedUserId Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required string ExternalId { get; init; }
    public AuthorizedUserStatus Status { get; private set; }
    public DateTimeOffset? ArchivedAt { get; private set; }
    public required ClientId ClientId { get; init; }

    internal Result Archive()
    {
        if (Status == AuthorizedUserStatus.Archived)
            return Result.Fail(new EntityAlreadyArchivedError(nameof(AuthorizedUser),
                "Authorized user is already archived"));

        Status = AuthorizedUserStatus.Archived;
        ArchivedAt = DateTimeOffset.UtcNow;
        return Result.Ok();
    }

    internal Result Suspend()
    {
        if (Status != AuthorizedUserStatus.Active)
            return Result.Fail(new EntityChangingStatusError(nameof(AuthorizedUser),
                $"Unable to suspend user with status {Status}"));

        Status = AuthorizedUserStatus.Suspended;
        return Result.Ok();
    }

    internal Result Reactivate()
    {
        if (Status != AuthorizedUserStatus.Suspended)
            return Result.Fail(new EntityChangingStatusError(nameof(AuthorizedUser), $"Unable to reactivate user with status {Status}"));

        Status = AuthorizedUserStatus.Active;
        return Result.Ok();
    }
}

[StronglyTypedId]
public readonly partial struct AuthorizedUserId;