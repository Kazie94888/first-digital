using SmartCoinOS.Domain.Orders;

namespace SmartCoinOS.Tests.Domain.OrderTests;

public static class RedeemTestExtensions
{
    public static RedeemOrder EnrichWithRsnReference(this RedeemOrder order)
    {
        order.AddRsnReference(new RsnReference("RSN-567"), BaseOrderTest.UserInfo);
        return order;
    }

    public static RedeemOrder EnrichWithTransactionExecution(this RedeemOrder order)
    {
        var dateOfSign = new DateTime(2024, 01, 05);
        var signature = new SafeSignature("0xA29773B3a967681322a20f55510767f9459646FA", "John Smith", new DateTimeOffset(dateOfSign));

        order.Executed(signature, BaseOrderTest.UserInfo);

        return order;
    }

    public static RedeemOrder EnrichWithSignature(this RedeemOrder order)
    {
        var dateOfSign = new DateTime(2024, 01, 05);
        var signature = new SafeSignature("0xA29773B3a967681322a20f55510767f9459646FA", "John Smith", new DateTimeOffset(dateOfSign));

        order.Signed("0x421558f7f5590b23442c41CE1e5382Bf901a890C", [signature], BaseOrderTest.UserInfo);

        return order;
    }

    public static RedeemOrder EnrichWithRedeemHash(this RedeemOrder order)
    {
        if (order.Type != OrderType.Redeem)
            throw new NotSupportedException();

        const string hash = "0x421558f7f5590b23442c41CE1e5382Bf901a890C";
        order.SetTransactionHash(hash, BaseOrderTest.UserInfo);

        return order;
    }

    public static RedeemOrder EnrichWithRedeemDepositConfirmation(this RedeemOrder order)
    {
        if (order.Type != OrderType.Redeem)
            throw new NotSupportedException();

        order.ConfirmDeposit(order.OrderedAmount, BaseOrderTest.UserInfo);

        return order;
    }
}
