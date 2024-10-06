using SmartCoinOS.Domain.Application;
using SmartCoinOS.Domain.Application.Events;
using SmartCoinOS.Domain.Results;

namespace SmartCoinOS.Tests.Domain.ApplicationTests;

public sealed class EntityParticularTests : BaseApplicationTest
{
    [Fact]
    public void AddEntityParticulars_WithValidData_Successfully()
    {
        var application = GetApplicationFromFactory();

        var result = application.SetEntityParticulars(CompanyLegalStructure.Other,
            "Lorem Ipsum is simply dummy text of the printing and typesetting industry",
            "123456789", DateOnly.FromDateTime(DateTime.UtcNow), "USA",
            [DefaultApplicationDocument], "Lorem Ipsum is simply dummy text", "10003", "NY", "USA",
            "New York", "email@gmail.com", "0356565698", UserInfo);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void AddEntityParticulars_WhenSuccess_ApplicationDocumentAddedEventIsRegistered()
    {
        var application = GetApplicationFromFactory();

        application.SetEntityParticulars(CompanyLegalStructure.Other,
            "Lorem Ipsum is simply dummy text of the printing and typesetting industry",
            "123456789", DateOnly.FromDateTime(DateTime.UtcNow), "USA",
            [DefaultApplicationDocument], "Lorem Ipsum is simply dummy text", "10003", "NY", "USA",
            "New York", "email@gmail.com", "0356565698", UserInfo);

        var eventEmitted = application.DomainEvents.Any(x => x is ApplicationDocumentAddedEvent);

        Assert.True(eventEmitted);
    }

    [Fact]
    public void AddEntityParticulars_WhenLegalStructureIsOtherAndStructureDetailsIsEmpty_ThrowException()
    {
        var application = GetApplicationFromFactory();

        var result = application.SetEntityParticulars(CompanyLegalStructure.Other,
            string.Empty,
            "123456789", DateOnly.FromDateTime(DateTime.UtcNow), "USA",
            [DefaultApplicationDocument], "Lorem Ipsum is simply dummy text",
            "10003", "NY", "USA",
            "New York", "email@gmail.com", "0356565698", UserInfo);

        Assert.True(result.IsFailed);
        Assert.True(result.HasError<EntityInvalidError>());
    }
}