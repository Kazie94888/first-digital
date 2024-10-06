using SmartCoinOS.Domain.Orders;

namespace SmartCoinOS.SmartCoinTool.DataSeed;

internal class FakeOrderNumberGenerator : IOrderNumberService
{
    private readonly HashSet<string> _consumed = [];

    public Task<OrderNumber> GetOrderNumberAsync(OrderType orderType, CancellationToken cancellationToken)
    {
        var orderNumber = OrderNumber.New(orderType);

        while (_consumed.Contains(orderNumber))
        {
            orderNumber = OrderNumber.New(orderType);
        }

        _consumed.Add(orderNumber);

        return Task.FromResult(orderNumber);
    }
}
