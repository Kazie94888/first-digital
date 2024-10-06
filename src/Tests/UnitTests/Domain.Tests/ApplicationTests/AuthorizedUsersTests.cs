using SmartCoinOS.Domain.Application.Events;
using SmartCoinOS.Domain.Results;

namespace SmartCoinOS.Tests.Domain.ApplicationTests;

public sealed class AuthorizedUsersTests : BaseApplicationTest
{
    [Fact]
    public void AddAuthorizedUsers_WithValidData_Successfully()
    {
        var application = GetApplicationFromFactory();

        var result = application.SetAuthorizedUsersForm("Join", "Smith", "userid",
            DefaultApplicationDocument, DefaultApplicationDocument, UserInfo);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void AddAuthorizedUsers_WhenUserAlreadyExisted_ThrowException()
    {
        var application = GetApplicationFromFactory();

        application.SetAuthorizedUsersForm("Join", "Smith", "userid", DefaultApplicationDocument, DefaultApplicationDocument, UserInfo);
        var result = application.SetAuthorizedUsersForm("Join", "Smith", "userid", DefaultApplicationDocument, DefaultApplicationDocument, UserInfo);

        Assert.True(result.IsFailed);
        Assert.True(result.HasError<EntityAlreadyExistsError>());
    }

    [Fact]
    public void AddAuthorizedUsers_WhenSuccess_ApplicationDocumentAddedEventIsRegistered()
    {
        var application = GetApplicationFromFactory();
        application.SetAuthorizedUsersForm("Join", "Smith", "userid",
            DefaultApplicationDocument,
            DefaultApplicationDocument,
            UserInfo);

        var eventEmitted = application.DomainEvents.Any(x => x is ApplicationDocumentAddedEvent);

        Assert.True(eventEmitted);
    }
}