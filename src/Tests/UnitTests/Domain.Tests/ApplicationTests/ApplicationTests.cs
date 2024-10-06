using FluentResults;
using SmartCoinOS.Domain.Application;
using SmartCoinOS.Domain.Application.Enums;
using SmartCoinOS.Domain.Application.Events;
using SmartCoinOS.Domain.Shared;

namespace SmartCoinOS.Tests.Domain.ApplicationTests;

public sealed class ApplicationTests : BaseApplicationTest
{
    [Fact]
    public void CreateApplication_WithValidData_Successfully()
    {
        var application = GetApplicationFromFactory();

        Assert.NotNull(application);
        Assert.Equal(LegalEntityName, application.LegalEntityName);
        Assert.Equal(UserInfo, application.CreatedBy);
    }

    [Fact]
    public void ValidateApplicationNumber_ApplicationNumberIsValid_ReturnsTrue()
    {
        var application = GetApplicationFromFactory();

        Assert.True(ApplicationNumber.IsValid(application.ApplicationNumber));
    }

    [Fact]
    public void ValidateApplicationNumber_WhenCreating_ApplicationNumberMustBeUnique()
    {
        var application = GetApplicationFromFactory();
        var anotherApplication = GetApplicationFromFactory();

        Assert.NotEqual(application.ApplicationNumber, anotherApplication.ApplicationNumber);
    }

    [Fact]
    public void ValidateApplicationNumber_ApplicationNumberIsNotValid_ReturnsFalse()
    {
        var applicationNumber = new ApplicationNumber { Value = "invalid application number" };

        Assert.False(ApplicationNumber.IsValid(applicationNumber));
    }

    [Fact]
    public void SubmitApplication_WhenSuccess_StatusChangedToInReview()
    {
        var application = GetApplication();
        var result = application.Submit(UserInfo);

        Assert.True(result.IsSuccess);
        Assert.Equal(ApplicationStatus.InReview, application.Status);
    }

    [Fact]
    public void SubmitApplication_WhenSuccess_ApplicationSubmittedEventIsRegistered()
    {
        var application = GetApplication();
        application.Submit(UserInfo);

        var eventEmitted = application.DomainEvents.Any(x => x is ApplicationSubmittedEvent);

        Assert.True(eventEmitted);
    }

    [Fact]
    public void ApproveApplication_WhenSuccess_StatusChangedToApproved()
    {
        var application = GetApplicationInReview();

        var result = application.Approve(UserInfo);

        Assert.True(result.IsSuccess);
        Assert.Equal(ApplicationStatus.Approved, application.Status);
    }

    [Fact]
    public void ApproveApplication_WhenSuccess_ApplicationApprovedEventIsRegistered()
    {
        var application = GetApplicationInReview();

        application.Approve(UserInfo);

        var eventEmitted = application.DomainEvents.Any(x => x is ApplicationApprovedEvent);
        Assert.True(eventEmitted);
    }

    [Fact]
    public void ApproveApplication_WhenSuccess_ApplicationApprovedAuditEventIsRegistered()
    {
        var application = GetApplicationInReview();

        application.Approve(UserInfo);

        var eventEmitted = application.DomainEvents.Any(x => x is ApplicationApprovedAuditEvent);
        Assert.True(eventEmitted);
    }

    [Fact]
    public void ApproveApplication_WhenStatusIsNotInReview_ThrowException()
    {
        var application = GetApplication();

        var result = application.Approve(UserInfo);

        Assert.True(result.IsFailed);
        Assert.True(result.HasError<IError>());
    }

    [Fact]
    public void ApproveApplication_WhenRequiredFormsAreNotCompleted_ThrowException()
    {
        var application = GetApplication();

        application.Submit(UserInfo);
        var result = application.Approve(UserInfo);

        Assert.True(result.IsFailed);
        Assert.True(result.HasError<IError>());
    }

    [Fact]
    public void ApproveApplication_WhenEntityParticularsAreNotVerified_ThrowException()
    {
        var application = GetApplication();

        application.Submit(UserInfo);
        var result = application.Approve(UserInfo);

        Assert.True(result.IsFailed);
        Assert.True(result.HasError<IError>());
    }

    [Fact]
    public void ApproveApplication_WhenAuthorizedUsersAreNotVerified_ThrowException()
    {
        var application = GetApplication();

        application.Submit(UserInfo);
        var result = application.Approve(UserInfo);

        Assert.True(result.IsFailed);
        Assert.True(result.HasError<IError>());
    }

    [Fact]
    public void ApproveApplication_WhenBusinessInfoIsNotVerified_ThrowException()
    {
        var application = GetApplication();

        application.Submit(UserInfo);
        var result = application.Approve(UserInfo);

        Assert.True(result.IsFailed);
        Assert.True(result.HasError<IError>());
    }

    [Fact]
    public void ApproveApplication_WhenBankAccountsAreNotVerified_ThrowException()
    {
        var application = GetApplication();

        application.Submit(UserInfo);
        var result = application.Approve(UserInfo);

        Assert.True(result.IsFailed);
        Assert.True(result.HasError<IError>());
    }

    [Fact]
    public void ApproveApplication_WhenDocumentsAreNotVerified_ThrowException()
    {
        var application = GetApplication();

        application.Submit(UserInfo);
        var result = application.Approve(UserInfo);

        Assert.True(result.IsFailed);
        Assert.True(result.HasError<IError>());
    }

    [Fact]
    public void RejectApplication_WhenSuccess_StatusChangedToRejected()
    {
        var application = GetApplicationInReview();

        var result = application.Reject(UserInfo);

        Assert.True(result.IsSuccess);
        Assert.Equal(ApplicationStatus.Rejected, application.Status);
    }

    [Fact]
    public void RejectApplication_WhenSuccess_ApplicationRejectedEventIsRegistered()
    {
        var application = GetApplicationInReview();

        application.Reject(UserInfo);

        var eventEmitted = application.DomainEvents.Any(x => x is ApplicationRejectedEvent);

        Assert.True(eventEmitted);
    }

    [Fact]
    public void RejectApplication_WhenStatusIsNotApproved_AndRejected_ThrowException()
    {
        var application = GetApplicationInReview();

        var successReject = application.Reject(UserInfo);
        var failedReject = application.Reject(UserInfo);

        Assert.True(successReject.IsSuccess);
        Assert.True(failedReject.IsFailed);
        Assert.True(failedReject.HasError<IError>());
    }

    [Fact]
    public void RemoveDocument_WhenFormSubmitDisabled_FailsWithError()
    {
        var application = GetApplication();

        var successReject = application.Reject(UserInfo);
        var failRemoveDoc = application.RemoveDocument(DocumentId.New());

        Assert.True(successReject.IsSuccess);
        Assert.True(failRemoveDoc.IsFailed);
    }

    [Fact]
    public void RemoveDocument_WhenDocumentNotFound_ReturnsOk()
    {
        var application = GetApplication();

        var canBeModified = application.CanSubmitForms();
        var tryRemoveDocument = application.RemoveDocument(DocumentId.New());

        Assert.True(canBeModified);
        Assert.True(tryRemoveDocument.IsSuccess);
    }

    [Fact]
    public void Archive_WhenSuccessful_SetsProperties()
    {
        var application = GetApplication();

        var archiveResult = application.Archive(UserInfo);
        var eventEmitted = application.DomainEvents.Any(x => x is ApplicationArchivedEvent);

        Assert.True(archiveResult.IsSuccess);
        Assert.True(application.Archived);
        Assert.True(eventEmitted);
    }
}