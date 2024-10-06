using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Domain.Orders;
using SmartCoinOS.Domain.Orders.Events;
using SmartCoinOS.Extensions.Fiql;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Orders;

public sealed class OrderAuditListQueryHandler : IQueryHandler<OrderAuditListQuery, InfoPagedList<OrderAuditListResponse>>
{
    private readonly ReadOnlyDataContext _context;

    public OrderAuditListQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<InfoPagedList<OrderAuditListResponse>>> Handle(OrderAuditListQuery request, CancellationToken cancellationToken)
    {
        var orderAuditsQuery = from audit in _context.AuditLogs
                               where audit.Event == nameof(OrderCreatedEvent)
                               select audit;

        var totalCount = await orderAuditsQuery.CountAsync(cancellationToken);
        var orderAudits = await orderAuditsQuery
                                    .SortAndPage(request)
                                    .ToListAsync(cancellationToken);

        var dtoModel = (from audit in orderAudits
                        orderby audit.Timestamp descending
                        group audit by new { Timestamp = DateOnly.FromDateTime(audit.Timestamp.DateTime) } into auditGroup
                        select new OrderAuditListResponse
                        {
                            Date = auditGroup.Key.Timestamp,
                            Data = auditGroup.Select(x => new OrderAuditItem
                            {
                                Date = x.Timestamp,
                                Description = x.EventDescription,
                                Type = x.GetParameter(nameof(Order.Type)),
                                Id = x.GetParameter(nameof(Order.OrderNumber))
                            }).ToList()
                        }).ToList();

        return Result.Ok(new InfoPagedList<OrderAuditListResponse>(dtoModel, totalCount, request.Page, request.Take));
    }
}