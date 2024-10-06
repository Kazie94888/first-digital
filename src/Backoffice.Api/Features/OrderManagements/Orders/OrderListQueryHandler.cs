using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Extensions.Fiql;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Orders;

internal sealed class OrderListQueryHandler : IQueryHandler<OrderListQuery, InfoPagedList<OrderListResponse>>
{
    private readonly ReadOnlyDataContext _context;

    public OrderListQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<InfoPagedList<OrderListResponse>>> Handle(OrderListQuery request,
        CancellationToken cancellationToken)
    {
        var ordersQuery = (
            from order in _context.Orders
            join client in _context.Clients on order.ClientId equals client.Id
            join ep in _context.EntityParticulars on client.Id equals ep.ClientId
            where !ep.Archived
            select new OrderListResponse
            {
                Id = order.Id,
                PrettyId = order.OrderNumber.Value,
                OrderType = order.Type,
                CreatedAt = order.CreatedAt,
                ClientName = ep.LegalEntityName,
                Progress = order.Status,
                ProcessingStatus = order.ProcessingStatus,
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
            }
        ).Filter(request);

        var totalCount = await ordersQuery.CountAsync(cancellationToken);
        var orders = await ordersQuery.SortAndPage(request)
            .ToListAsync(cancellationToken);

        return Result.Ok(new InfoPagedList<OrderListResponse>(orders, totalCount, request.Page, request.Take));
    }
}
