using SmartCoinOS.Domain.Application;
using SmartCoinOS.Domain.Application.Events;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Results;

namespace SmartCoinOS.Tests.Domain.ApplicationTests;

public sealed class BankAccountsTests : BaseApplicationTest
{
    private readonly SmartTrustBank _smartTrustBank = new(1234, 12);

    [Fact]
    public void AddBankAccounts_WithValidData_Successfully()
    {
        var application = GetApplicationFromFactory();

        var result = application.SetBankAccounts("beneficiary", "bankName", "iban", "swift", _smartTrustBank,
            SampleAddress, [DefaultApplicationDocument], UserInfo);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void AddBankAccounts_WhenBankAccountAlreadyExisted_ThrowException()
    {
        var application = GetApplicationFromFactory();

        var beneficiary = "beneficiary";
        var bankName = "bankName";
        var iban = "iban";
        var swift = "swift";

        var documents = new List<ApplicationDocument> { DefaultApplicationDocument };

        application.SetBankAccounts(beneficiary, bankName, iban, swift, _smartTrustBank,
            SampleAddress, documents, UserInfo);

        var result = application.SetBankAccounts(beneficiary, bankName, iban, swift, _smartTrustBank,
            SampleAddress, documents, UserInfo);

        Assert.True(result.IsFailed);
        Assert.True(result.HasError<EntityAlreadyExistsError>());
    }

    [Fact]
    public void AddBankAccounts_WhenSuccess_ApplicationDocumentAddedEventIsRegistered()
    {
        var application = GetApplicationFromFactory();
        application.SetBankAccounts("beneficiary", "bankName", "iban", "swift",
            _smartTrustBank, SampleAddress, [DefaultApplicationDocument], UserInfo);

        var eventEmitted = application.DomainEvents.Any(x => x is ApplicationDocumentAddedEvent);

        Assert.True(eventEmitted);
    }

    [Fact]
    public void ClearBankAccounts_WithValidData_Successfully()
    {
        var application = GetApplicationFromFactory();
        application.SetBankAccounts("beneficiary", "bankName", "iban", "swift", _smartTrustBank,
            SampleAddress, [DefaultApplicationDocument], UserInfo);

        application.ClearBankAccounts();

        Assert.Empty(application.BankAccounts.BankRecords);
    }
}
