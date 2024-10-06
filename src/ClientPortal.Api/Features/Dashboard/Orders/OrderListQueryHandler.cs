using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.ClientPortal.Api.Base;
using SmartCoinOS.ClientPortal.Api.Base.Dto;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Domain.Enums;
using SmartCoinOS.Extensions.Fiql;
using SmartCoinOS.Persistence;

namespace SmartCoinOS.ClientPortal.Api.Features.Dashboard.Orders;

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
        var ordersQuery = _context.Orders
            .Where(order => order.ClientId == request.UserInfo.ClientId)
            .Select(order => new OrderListResponse
            {
                Id = order.Id,
                PrettyId = order.OrderNumber.Value,
                OrderType = order.Type,
                CreatedAt = order.CreatedAt,
                Progress = order.Status,
                OrderAmount = order.OrderedAmount,
                CreatedBy = new UserInfoDto
                {
                    Id = order.CreatedBy.Id,
                    Username = order.CreatedBy.Type == UserInfoType.ClientPortal
                        ? order.CreatedBy.Username
                        : ApplicationConstants.AdminUsername,
                    ClientId = order.ClientId
                }
            }).Filter(request);

        var totalCount = await ordersQuery.CountAsync(cancellationToken);
        var orders = await ordersQuery.SortAndPage(request).ToListAsync(cancellationToken);

        return Result.Ok(new InfoPagedList<OrderListResponse>(orders, totalCount, request.Page, request.Take));
    }
}
