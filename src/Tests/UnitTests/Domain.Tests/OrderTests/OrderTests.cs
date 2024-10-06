using SmartCoinOS.Domain.Orders;
using SmartCoinOS.Domain.Orders.Events;

namespace SmartCoinOS.Tests.Domain.OrderTests;

public sealed class OrderTests : BaseOrderTest
{
    [Theory]
    [InlineData(OrderType.Mint)]
    [InlineData(OrderType.Redeem)]
    public void Order_WhenCreatedFromFactory_EmitsCreatedEvent(OrderType orderType)
    {
        Order order = orderType switch
        {
            OrderType.Mint => GetMintOrderFromFactory(),
            _ => GetRedeemOrderFromFactory()
        };

        var registeredDomainEvents = order.DomainEvents.Count(x => x is OrderCreatedEvent);

        Assert.NotNull(order);
        Assert.Equal(1, registeredDomainEvents);
    }

    [Theory]
    [InlineData("M5E-DD0")]
    [InlineData("R78-VG6")]
    [InlineData("MM8-KB8")]
    [InlineData("MF85-5LJKB3")]
    public void OrderNumber_IsValid_CompletesSuccessfully(string givenNumber)
    {
        var orderNumber = new OrderNumber { Value = givenNumber };

        var orderNumberIsValid = OrderNumber.IsValid(orderNumber);

        Assert.True(orderNumberIsValid);
    }

    [Fact]
    public void OrderNumber_IsValid_FalseWhenGivenUnknownType()
    {
        var orderNumber = new OrderNumber { Value = "LMM8-KB8" };

        var isValid = OrderNumber.IsValid(orderNumber);

        Assert.False(isValid);
    }

    [Fact]
    public void OrderNumber_IsValid_FalseWhenCheckDigitMissing()
    {
        var orderNumber = new OrderNumber { Value = "LMM8-KB8F" };

        var isValid = OrderNumber.IsValid(orderNumber);

        Assert.False(isValid);
    }
}
