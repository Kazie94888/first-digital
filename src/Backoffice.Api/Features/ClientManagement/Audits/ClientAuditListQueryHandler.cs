using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Extensions.Fiql;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Audits;

internal sealed class GetClientAuditsHandler
    : IQueryHandler<ClientAuditListQuery, InfoPagedList<ClientAuditListResponse>>
{
    private readonly ReadOnlyDataContext _context;

    public GetClientAuditsHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<InfoPagedList<ClientAuditListResponse>>> Handle(
        ClientAuditListQuery request,
        CancellationToken cancellationToken)
    {
        var clientId = request.ClientId.Value.ToString();

        var auditsQuery = (
            from audit in _context.AuditLogs
            where audit.Parameters.Any(p => p.Name == GlobalConstants.AuditParameters.ClientId && p.Value == clientId)
            orderby audit.Timestamp descending
            select new
            {
                audit.Id,
                audit.Event,
                audit.Timestamp,
                Description = audit.EventDescription,
                audit.Parameters,
                audit.CreatedBy
            }
        ).Filter(request);

        var totalCount = await auditsQuery.CountAsync(cancellationToken);
        var auditsResult = await auditsQuery.SortAndPage(request).ToListAsync(cancellationToken);

        var audits = auditsResult.Select(x => new ClientAuditListResponse()
        {
            Id = x.Id,
            Event = x.Event,
            Description = x.Description,
            Timestamp = x.Timestamp,
            Parameters = x.Parameters.Select(p => new AuditLogParameterDto()
            {
                Name = p.Name,
                Value = p.Value
            }),
            InitiatedBy = new UserInfoDto()
            {
                Id = x.CreatedBy.Id,
                Type = x.CreatedBy.Type,
                Username = x.CreatedBy.Username
            }
        }).ToList();

        return new InfoPagedList<ClientAuditListResponse>(audits, totalCount, request.Page, request.Take);
    }
}
