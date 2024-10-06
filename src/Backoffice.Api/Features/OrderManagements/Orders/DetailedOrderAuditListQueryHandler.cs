using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Domain.Orders.Events;
using static SmartCoinOS.Domain.Base.GlobalConstants;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Orders;

public sealed class DetailedOrderAuditListQueryHandler : IQueryHandler<DetailedOrderAuditListQuery, InfoList<DetailedOrderAuditListQueryResponse>>
{
    private readonly ReadOnlyDataContext _context;

    public DetailedOrderAuditListQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<InfoList<DetailedOrderAuditListQueryResponse>>> Handle(DetailedOrderAuditListQuery request, CancellationToken cancellationToken)
    {
        var orderEvents = OrderAuditEventBase.OrderEventNames;

        var auditsList = await (from audit in _context.AuditLogs
                                where orderEvents.Contains(audit.Event)
                                        && audit.Parameters.Any(p => p.Name == AuditParameters.OrderId && p.Value == request.OrderId.ToString())
                                orderby audit.Timestamp descending
                                select new DetailedOrderAuditListQueryResponse
                                {
                                    Id = audit.Id,
                                    Event = audit.Event,
                                    CreatedAt = audit.Timestamp,
                                    Description = audit.EventDescription
                                }).ToListAsync(cancellationToken);

        return Result.Ok(new InfoList<DetailedOrderAuditListQueryResponse>(auditsList));
    }
}