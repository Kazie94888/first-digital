using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Results;

namespace SmartCoinOS.Tests.Domain.ApplicationTests;

public sealed class WalletsTests : BaseApplicationTest
{
    [Fact]
    public void AddWallets_WithValidData_Successfully()
    {
        var application = GetApplicationFromFactory();

        var result = application.SetWallets(BlockchainNetwork.BnbSmartchain, "walletAddress");

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void AddWallets_WhenWalletAlreadyExisted_ThrowException()
    {
        var application = GetApplicationFromFactory();

        var blockchainNetwork = BlockchainNetwork.BnbSmartchain;
        var walletAddress = "walletAddress";

        application.SetWallets(blockchainNetwork, walletAddress);

        var result = application.SetWallets(blockchainNetwork, walletAddress);

        Assert.True(result.IsFailed);
        Assert.True(result.HasError<EntityAlreadyExistsError>());
    }

    [Fact]
    public void ClearWallets_WithValidData_Successfully()
    {
        var application = GetApplicationFromFactory();

        application.SetWallets(BlockchainNetwork.BnbSmartchain, "walletAddress");

        application.ClearWallets();

        Assert.Empty(application.Wallets.Wallets);
    }
}