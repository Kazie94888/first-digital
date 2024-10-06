using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.AuditLogs;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Domain.Enums;
using SmartCoinOS.Domain.Orders;
using SmartCoinOS.Domain.Orders.Events;
using SmartCoinOS.Domain.SeedWork;
using SmartCoinOS.Persistence;
using static SmartCoinOS.Domain.Base.GlobalConstants;

namespace SmartCoinOS.ClientPortal.Api.Features.OrderManagements.Orders;

internal sealed class
    DetailedOrderAuditListQueryHandler : IQueryHandler<DetailedOrderAuditListQuery, InfoList<OrderAuditItem>>
{
    private readonly ReadOnlyDataContext _context;

    public DetailedOrderAuditListQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<InfoList<OrderAuditItem>>> Handle(DetailedOrderAuditListQuery request,
        CancellationToken cancellationToken)
    {
        var orderEvents = OrderAuditEventBase.OrderEventNames;


        var auditsList = await (from audit in _context.AuditLogs
            where orderEvents.Contains(audit.Event)
                  && audit.Parameters.Any(p =>
                      p.Name == AuditParameters.OrderId && p.Value == request.OrderId.ToString())
                  && audit.Parameters.Any(p =>
                      p.Name == AuditParameters.ClientId && p.Value == request.UserInfo.ClientId.ToString())
            orderby audit.Timestamp descending
            select new
            {
                audit.Id,
                audit.Event,
                audit.CreatedBy,
                CreatedAt = audit.Timestamp,
                Description = audit.EventDescription,
                audit.Parameters
            }).ToListAsync(cancellationToken);

        var eventsToObfusckate = new List<string>
        {
            nameof(MintTransactionExecutedEvent),
            nameof(RedeemTransactionExecutedEvent),
            nameof(SigningOccurredEvent),
            nameof(SigningInitiatedEvent)
        };

        var auditEvents = new List<OrderAuditItem>();

        foreach (var item in auditsList)
        {
            var msg = item.Description;
            if (eventsToObfusckate.Contains(item.Event))
                msg = ObfusckateSignerAlias(item.Description, item.CreatedBy, item.Parameters);

            var orderAuditItem = new OrderAuditItem
            {
                Id = item.Id,
                Description = msg,
                Event = item.Event,
                CreatedAt = item.CreatedAt
            };
            auditEvents.Add(orderAuditItem);
        }

        return Result.Ok(new InfoList<OrderAuditItem>(auditEvents));
    }

    private string ObfusckateSignerAlias(string auditDescription, UserInfo createdBy,
        IReadOnlyList<AuditLogParameter> parameters)
    {
        var obfusckatedMessage = auditDescription;

        if (createdBy.Type == UserInfoType.BackOffice)
        {
            var signer =
                parameters.FirstOrDefault(param => param.Name.Equals(nameof(SafeSignature.Alias)));
            if (signer is not null)
                obfusckatedMessage = obfusckatedMessage.Replace(signer.Value, "Admin Member");
        }

        return obfusckatedMessage;
    }
}
