using SmartCoinOS.Domain.Application;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Results;

namespace SmartCoinOS.Tests.Domain.ClientTests;
public class ClientStatusTests : BaseClientTest
{
    [Fact]
    public void Client_WhenStatusIsInactive_StatusCantBeChanged()
    {
        var client = GetClientFromFactory();

        var deactivateResult = client.Suspend(permanent: true, UserInfo);
        var activateResult = client.Activate(UserInfo);
        var dormantResult = client.Dormant(UserInfo);
        var temporarlySuspendResult = client.Suspend(permanent: false, UserInfo);

        Assert.True(deactivateResult.IsSuccess);

        Assert.True(activateResult.IsFailed);
        Assert.True(dormantResult.IsFailed);
        Assert.True(temporarlySuspendResult.IsFailed);
    }

    [Theory]
    [InlineData(ClientStatus.Dormant)]
    [InlineData(ClientStatus.TemporarlySuspended)]
    [InlineData(ClientStatus.Inactive)]
    public void Client_WhenStatusDoesntAllow_CantModifyData(ClientStatus targetState)
    {
        var client = GetClientFromFactory();
        var suspendResult = targetState switch
        {
            ClientStatus.Dormant => client.Dormant(UserInfo),
            ClientStatus.TemporarlySuspended => client.Suspend(false, UserInfo),
            ClientStatus.Inactive => client.Suspend(true, UserInfo),
            _ => throw new ArgumentException("Invalid method argument", nameof(targetState))
        };

        var entityDetailsResult = client.EditEntityParticular("JPMorgan Junior",
                                    "USA",
                                    new DateOnly(2024, 1, 1),
                                    CompanyLegalStructure.SoleProprietorship,
                                    "FS54548",
                                    UserInfo);



        Assert.True(suspendResult.IsSuccess);
        Assert.True(entityDetailsResult.HasError<EntityChangingStatusError>());
    }

    [Theory]
    [InlineData(ClientStatus.Dormant)]
    [InlineData(ClientStatus.TemporarlySuspended)]
    [InlineData(ClientStatus.Inactive)]
    public void Client_WhenStatusDoesntAllow_BankIsNotUsable(ClientStatus targetState)
    {
        var client = GetClientFromFactory().EnrichWithBankAccount();

        var suspendResult = targetState switch
        {
            ClientStatus.Dormant => client.Dormant(UserInfo),
            ClientStatus.TemporarlySuspended => client.Suspend(false, UserInfo),
            ClientStatus.Inactive => client.Suspend(true, UserInfo),
            _ => throw new ArgumentException("Invalid method argument", nameof(targetState))
        };

        var archiveResult = client.ArchiveBankAccount(client.BankAccounts[0].Id, false, UserInfo);
        
        var bankAccountRecord = new CreatedClientBankAccountRecord
        {
            Beneficiary = "Jason Smith",
            Iban = "XK788198071070721032",
            BankName = "Mega Corp",
            SwiftCode = "LIAICNT1",
            Alias = "Velda Davis",
            SmartTrustBank = new SmartTrustBank(9690, 9690),
            Address = SampleAddress
        };
        
        var addBankResult = client.AddBankAccount(bankAccountRecord, UserInfo);

        Assert.True(suspendResult.IsSuccess);
        Assert.True(archiveResult.HasError<EntityChangingStatusError>());
        Assert.True(addBankResult.HasError<EntityChangingStatusError>());
    }

    [Theory]
    [InlineData(ClientStatus.Dormant)]
    [InlineData(ClientStatus.TemporarlySuspended)]
    [InlineData(ClientStatus.Inactive)]
    public void Client_WhenStatusDoesntAllow_WalletIsNotUsable(ClientStatus targetState)
    {
        var client = GetClientFromFactory().EnrichWithWallet();

        var suspendResult = targetState switch
        {
            ClientStatus.Dormant => client.Dormant(UserInfo),
            ClientStatus.TemporarlySuspended => client.Suspend(false, UserInfo),
            ClientStatus.Inactive => client.Suspend(true, UserInfo),
            _ => throw new ArgumentException("Invalid method argument", nameof(targetState))
        };

        var archiveWalletResult = client.ArchiveWallet(client.Wallets[0].Id, false, UserInfo);
        var addWalletResult = client.AddWallet("0xa444f5ea0ba59494ce839613ffhba74279589269", BlockchainNetwork.SepoliaTestnet, "Wallet From UnitTests", UserInfo);


        Assert.True(suspendResult.IsSuccess);
        Assert.True(archiveWalletResult.HasError<EntityChangingStatusError>());
        Assert.True(addWalletResult.HasError<EntityChangingStatusError>());
    }

    [Theory]
    [InlineData(ClientStatus.Dormant)]
    [InlineData(ClientStatus.TemporarlySuspended)]
    [InlineData(ClientStatus.Inactive)]
    public void Client_WhenStatusDoesntAllow_AuthorizedUsersAreNotUsable(ClientStatus targetState)
    {
        var client = GetClientFromFactory().EnrichWithAuthorizedUser();

        var suspendResult = targetState switch
        {
            ClientStatus.Dormant => client.Dormant(UserInfo),
            ClientStatus.TemporarlySuspended => client.Suspend(false, UserInfo),
            ClientStatus.Inactive => client.Suspend(true, UserInfo),
            _ => throw new ArgumentException("Invalid method argument", nameof(targetState))
        };

        var addUserResult = client.AddAuthorizedUser("Not", "Allowed", "user@company.com", Guid.Empty.ToString(), UserInfo);
        var archiveResult = client.ArchiveAuthorizedUser(client.AuthorizedUsers[0].Id, UserInfo);
        var reactivateResult = client.ReactiveAuthorizedUser(client.AuthorizedUsers[0].Id, UserInfo);
        var suspendUserResult = client.SuspendAuthorizedUser(client.AuthorizedUsers[0].Id, UserInfo);

        Assert.True(suspendResult.IsSuccess);
        Assert.True(addUserResult.HasError<EntityChangingStatusError>());
        Assert.True(archiveResult.HasError<EntityChangingStatusError>());
        Assert.True(reactivateResult.HasError<EntityChangingStatusError>());
        Assert.True(suspendUserResult.HasError<EntityChangingStatusError>());
    }

    [Theory]
    [InlineData(ClientStatus.Dormant)]
    [InlineData(ClientStatus.TemporarlySuspended)]
    [InlineData(ClientStatus.Inactive)]
    public void Client_WhenStatusDoesntAllow_FdtAccountCantBeAdded(ClientStatus targetState)
    {
        var client = GetClientFromFactory().EnrichWithAuthorizedUser();

        var suspendResult = targetState switch
        {
            ClientStatus.Dormant => client.Dormant(UserInfo),
            ClientStatus.TemporarlySuspended => client.Suspend(false, UserInfo),
            ClientStatus.Inactive => client.Suspend(true, UserInfo),
            _ => throw new ArgumentException("Invalid method argument", nameof(targetState))
        };

        var addAccountResult = client.AddFdtAccount("Binanciota", "001101001101", "Not made up trading platform", UserInfo);

        Assert.True(suspendResult.IsSuccess);
        Assert.True(addAccountResult.HasError<EntityChangingStatusError>());
    }

    [Fact]
    public void Client_WhenParticularEdited_PreviousVersionsArchived()
    {
        var client = GetClientFromFactory();

        string companyName = "new name", regNumber = "RGB-1234", countryOfInc = "Italy";
        var dateOfInc = new DateOnly(2030, 01, 01);

        var editResult = client.EditEntityParticular(companyName,
                                                     regNumber,
                                                     dateOfInc,
                                                     CompanyLegalStructure.PrivateLimitedCompany,
                                                     countryOfInc,
                                                     UserInfo);


        var onlyOneUnarchived = client.EntityParticulars.Count(x => !x.Archived) == 1;
        var lastOneUnarchived = client.CurrentEntityParticular.Id == editResult.Value.Id;

        Assert.True(editResult.IsSuccess);
        Assert.True(onlyOneUnarchived);
        Assert.True(lastOneUnarchived);
    }
}
