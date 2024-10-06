using SmartCoinOS.Domain.Application;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Tests.Domain.ClientTests;

public abstract class BaseClientTest
{
    internal static readonly UserInfo UserInfo = new()
    {
        Id = Guid.NewGuid(), Type = SmartCoinOS.Domain.Enums.UserInfoType.BackOffice, Username = "DomainUnitTest"
    };

    internal static readonly AddressRecord SampleAddress = new()
    {
        Country = "United Kingdom", City = "West Lornaport", PostalCode = "97048", Street = "1121 1st street"
    };

    internal static Client GetClientFromFactory(bool verifyEntityParticular = false)
    {
        var builder = new ClientBuilder(UserInfo);

        builder.AddEntityParticulars("JPMorgan Chase",
            "ABA487R7",
            new DateOnly(2024, 1, 1),
            CompanyLegalStructure.SoleProprietorship,
            null,
            "USA");

        builder.AddAddress("USA",
            "NY",
            "10003",
            "New York",
            "2nd Street");
        builder.AddContact("fred@morgan.ai", "3215458524");
        builder.AddMetadata("XUY-000-ASD", null);

        var clientAggregate = builder.Build();

        if (verifyEntityParticular)
            clientAggregate.VerifyEntityParticular(clientAggregate.CurrentEntityParticular.Id, UserInfo);

        return clientAggregate;
    }
}
