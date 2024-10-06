using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Domain.Exceptions;
using SmartCoinOS.Domain.Orders;
using SmartCoinOS.Persistence;

namespace SmartCoinOS.Infrastructure.Services;
internal class OrderNumberService : IOrderNumberService
{
    private readonly DataContext _dataContext;
    private const int _maxAttempts = 10;

    public OrderNumberService(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<OrderNumber> GetOrderNumberAsync(OrderType orderType, CancellationToken cancellationToken)
    {
        var attempt = 0;
        var orderNumber = OrderNumber.New(orderType);
        var existsAlready = await _dataContext.Orders.AnyAsync(o => o.OrderNumber == orderNumber, cancellationToken);

        while (existsAlready)
        {
            attempt++;
            if (attempt > _maxAttempts)
                throw new RetriesExceededException("Attempts exceeded while trying to find a unique order number.");

            orderNumber = OrderNumber.New(orderType);
            existsAlready = await _dataContext.Orders.AnyAsync(o => o.OrderNumber == orderNumber, cancellationToken);
        }

        return orderNumber;
    }
}
