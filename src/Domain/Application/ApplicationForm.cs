using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Application;

public abstract class ApplicationForm
{
    internal abstract bool IsVerified();

    internal abstract void RequireAdditionalInfo();

    internal abstract Result Verify(string segmentOrRecord, UserInfo userInfo);
}
