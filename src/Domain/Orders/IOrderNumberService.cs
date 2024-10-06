namespace SmartCoinOS.Domain.Orders;

public interface IOrderNumberService
{
    Task<OrderNumber> GetOrderNumberAsync(OrderType orderType, CancellationToken cancellationToken);
}
