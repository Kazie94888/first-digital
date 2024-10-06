using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Clients;

public abstract class ClientAsset : Entity
{
    public bool Archived { get; protected set; }
    public DateTimeOffset? ArchivedAt { get; protected set; }
    public EntityVerificationStatus VerificationStatus { get; internal set; }

    private Result ChangeStatus(EntityVerificationStatus newStatus)
    {
        if (Archived)
            return new EntityAlreadyArchivedError(nameof(ClientAsset), "Client asset is archived. No further changes are permitted.");

        if (VerificationStatus == EntityVerificationStatus.Declined)
            return new EntityChangingStatusError(nameof(ClientAsset), "Client asset status is declined. No further changes are permitted.");

        if (VerificationStatus == newStatus)
            return new EntityChangingStatusError(nameof(ClientAsset), $"Client asset status is already {newStatus}.");

        VerificationStatus = newStatus;

        return Result.Ok();
    }

    public Result Verify() => ChangeStatus(EntityVerificationStatus.Verified);
    public Result Decline() => ChangeStatus(EntityVerificationStatus.Declined);

    internal Result Archive()
    {
        if (Archived)
            return new EntityAlreadyArchivedError(nameof(ClientAsset), "Asset is already archived.");

        Archived = true;
        ArchivedAt = DateTimeOffset.UtcNow;

        return Result.Ok();
    }
}