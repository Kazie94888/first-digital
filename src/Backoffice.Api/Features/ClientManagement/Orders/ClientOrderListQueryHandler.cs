using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Extensions.Fiql;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Orders;

internal sealed class
    ClientOrderListQueryHandler : IQueryHandler<ClientOrderListQuery, InfoPagedList<ClientOrderListResponse>>
{
    private readonly ReadOnlyDataContext _context;

    public ClientOrderListQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<InfoPagedList<ClientOrderListResponse>>> Handle(ClientOrderListQuery request,
        CancellationToken cancellationToken)
    {
        var ordersQuery = _context.Orders
            .Where(order => order.ClientId == request.ClientId)
            .Select(order => new ClientOrderListResponse
            {
                Id = order.Id,
                PrettyId = order.OrderNumber.Value,
                OrderType = order.Type,
                CreatedAt = order.CreatedAt,
                Progress = order.Status,
                Amount =
                    order.ActualAmount == null
                        ? null
                        : new MoneyDto
                        {
                            Amount = order.ActualAmount.Amount,
                            AssetSymbol = order.ActualAmount.Currency
                        },
                AmountOrdered = new MoneyDto
                {
                    Amount = order.OrderedAmount.Amount,
                    AssetSymbol = order.OrderedAmount.Currency
                }
            })
            .Filter(request);

        var totalCount = await ordersQuery.CountAsync(cancellationToken);
        var orders = await ordersQuery.SortAndPage(request).ToListAsync(cancellationToken);

        return Result.Ok(new InfoPagedList<ClientOrderListResponse>(orders, totalCount, request.Page, request.Take));
    }
}
