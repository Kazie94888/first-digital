using SmartCoinOS.Domain.Results;

namespace SmartCoinOS.Tests.Domain.ApplicationTests;

public sealed class FdtAccountsTests : BaseApplicationTest
{

    [Fact]
    public void AddFdtAccounts_WithValidData_Successfully()
    {
        var application = GetApplicationFromFactory();

        var result = application.SetFdtAccounts("clientId", "accountNumber");

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void AddFdtAccounts_WhenFdtAccountAlreadyExisted_ThrowException()
    {
        var application = GetApplicationFromFactory();

        application.SetFdtAccounts("clientId", "accountNumber");

        var result = application.SetFdtAccounts("clientId", "accountNumber");

        Assert.True(result.IsFailed);
        Assert.True(result.HasError<EntityAlreadyExistsError>());
    }

    [Fact]
    public void ClearFdtAccounts_WithValidData_Successfully()
    {
        var application = GetApplicationFromFactory();

        application.SetFdtAccounts("clientId", "accountNumber");

        application.ClearFdtAccounts();

        Assert.Empty(application.FdtAccounts.FdtAccounts);
    }
}