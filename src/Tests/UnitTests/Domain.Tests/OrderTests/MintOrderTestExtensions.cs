using SmartCoinOS.Domain.Orders;

namespace SmartCoinOS.Tests.Domain.OrderTests;

public static class MintOrderTestExtensions
{
    public static MintOrder EnrichWithRsnReference(this MintOrder order)
    {
        order.AddRsnReference(new RsnReference("RSN-567"), BaseOrderTest.UserInfo);
        return order;
    }

    public static MintOrder EnrichWithRsnAmount(this MintOrder order)
    {
        order.AddRsnAmount(new Money { Amount = 1234, Currency = "FDUSD" }, BaseOrderTest.UserInfo);
        return order;
    }

    public static MintOrder EnrichWithTransactionExecution(this MintOrder order)
    {
        var dateOfSign = new DateTime(2024, 01, 05);
        var signature = new SafeSignature("0xA29773B3a967681322a20f55510767f9459646FA", "John Smith", new DateTimeOffset(dateOfSign));

        order.Executed(signature, BaseOrderTest.UserInfo);

        return order;
    }

    public static MintOrder EnrichWithSignature(this MintOrder order)
    {
        var dateOfSign = new DateTime(2024, 01, 05);
        var signature = new SafeSignature("0xA29773B3a967681322a20f55510767f9459646FA", "John Smith", new DateTimeOffset(dateOfSign));

        order.Signed("0x421558f7f5590b23442c41CE1e5382Bf901a890C", [signature], BaseOrderTest.UserInfo);

        return order;
    }
}
