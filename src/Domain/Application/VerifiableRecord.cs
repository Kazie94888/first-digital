using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Application;

public abstract record VerifiableRecord
{
    protected VerifiableRecord() { }

    protected VerifiableRecord(UserInfo? verifiedBy, DateTimeOffset? verifiedAt)
    {
        VerifiedBy = verifiedBy;
        VerifiedAt = verifiedAt;
    }

    public UserInfo? VerifiedBy { get; private set; }
    public DateTimeOffset? VerifiedAt { get; private set; }

    internal Result Verify(UserInfo userInfo)
    {
        if (VerifiedBy is not null)
            return new EntityChangingStatusError(nameof(VerifiedBy), "Entity is already verified");

        VerifiedBy = userInfo;
        VerifiedAt = DateTimeOffset.UtcNow;

        return Result.Ok();
    }

    internal void RemoveVerificationFlag()
    {
        VerifiedBy = null;
        VerifiedAt = null;
    }

    public bool IsVerified()
    {
        return VerifiedBy is not null && VerifiedAt.HasValue;
    }
}
