using SmartCoinOS.Domain.Orders;

namespace SmartCoinOS.Tests.Domain.OrderTests;

public sealed class MintOrderRsnTests : BaseOrderTest
{
    [Fact]
    public void AddRsnReference_WhenOk_SetsOrderProperties()
    {
        var order = GetMintOrderFromFactory();
        var reference = new RsnReference("A1B2C3");

        var result = order.AddRsnReference(reference, UserInfo);

        Assert.True(result.IsSuccess);
        Assert.Equal(reference, order.RsnReference);
        Assert.Equal(OrderProcessingStatus.RsnReferenceCreated, order.ProcessingStatus);
    }

    [Fact]
    public void AddRsnAmount_WhenNotMint_FailsWithError()
    {
        var order = GetMintOrderFromFactory();

        var result = order.AddRsnAmount(new Money() { Amount = 50_000, Currency = "FDUSD" }, UserInfo);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public void AddRsnAmount_WhenOk_SetsOrderProperties()
    {
        var order = GetMintOrderFromFactory()
                        .EnrichWithRsnReference();

        var amount = new Money() { Amount = 50_000, Currency = "FDUSD" };

        var result = order.AddRsnAmount(amount, UserInfo);

        Assert.True(result.IsSuccess);
        Assert.Equal(amount, order.ActualAmount);
        Assert.Equal(OrderStatus.InProgress, order.Status);
        Assert.Equal(OrderProcessingStatus.RsnAmountAdded, order.ProcessingStatus);
    }
}
