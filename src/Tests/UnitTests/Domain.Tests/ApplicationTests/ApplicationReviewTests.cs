using SmartCoinOS.Domain.Application;
using SmartCoinOS.Domain.Application.Enums;
using SmartCoinOS.Domain.Application.Events;

namespace SmartCoinOS.Tests.Domain.ApplicationTests;

public sealed class ApplicationReviewTests : BaseApplicationTest
{
    [Fact]
    public void AdditionalInfoRequired_WhenStatusNotInReview_ReturnsError()
    {
        var application = GetApplication();

        var result = application.AdditionalInfoRequired(ApplicationFormType.EntityParticulars, UserInfo);

        Assert.True(result.IsFailed);
    }

    [Theory]
    [InlineData(ApplicationFormType.EntityParticulars)]
    [InlineData(ApplicationFormType.BusinessInfo)]
    [InlineData(ApplicationFormType.AuthorizedUsers)]
    [InlineData(ApplicationFormType.BankAccounts)]
    [InlineData(ApplicationFormType.Wallets)]
    [InlineData(ApplicationFormType.FdtAccounts)]
    [InlineData(ApplicationFormType.Documents)]
    public void AdditionalInfoRequired_WhenRequiredEntityParticulars_ClearsVerificationState(ApplicationFormType formType)
    {
        var application = GetApplicationInReview();
        var formUnderTest = GetFormByType(application, formType);

        var isOriginallyVerified = FormIsVerified(formUnderTest);
        var requireMoreInfo = application.AdditionalInfoRequired(formType, UserInfo);
        var isStillVerified = FormIsVerified(formUnderTest);
        var statusAsExpected = application.Status == ApplicationStatus.AdditionalInformationRequired;
        var eventNotificationEmitted = application.DomainEvents.Any(e => e is ApplicationAdditionalInfoRequiredEvent);

        Assert.True(isOriginallyVerified);
        Assert.True(requireMoreInfo.IsSuccess);
        Assert.False(isStillVerified);
        Assert.True(statusAsExpected);
        Assert.True(eventNotificationEmitted);
    }

    private static ApplicationForm? GetFormByType(Application application, ApplicationFormType type)
    {
        return type switch
        {
            ApplicationFormType.EntityParticulars => application.EntityParticulars,
            ApplicationFormType.BusinessInfo => application.BusinessInfo,
            ApplicationFormType.AuthorizedUsers => application.AuthorizedUsers,
            ApplicationFormType.BankAccounts => application.BankAccounts,
            ApplicationFormType.Wallets => application.Wallets,
            ApplicationFormType.FdtAccounts => application.FdtAccounts,
            ApplicationFormType.Documents => application.Documents,

            _ => throw new ArgumentNullException(nameof(type))
        };
    }

    private static bool FormIsVerified(ApplicationForm? applicationForm)
    {
        if (applicationForm == null)
            return false;

        return applicationForm.IsVerified();
    }
}