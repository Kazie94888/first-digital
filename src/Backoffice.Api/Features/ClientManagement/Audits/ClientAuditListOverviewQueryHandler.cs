using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Base;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Audits;

internal sealed class ClientAuditListOverviewQueryHandler : IQueryHandler<ClientAuditListOverviewQuery, InfoList<ClientAuditListOverviewResponse>>
{
    private readonly ReadOnlyDataContext _context;
    private const int _lastEventsToTake = 5;

    public ClientAuditListOverviewQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<InfoList<ClientAuditListOverviewResponse>>> Handle(ClientAuditListOverviewQuery request, CancellationToken cancellationToken)
    {
        var clientId = request.ClientId.Value.ToString();

        var auditsQuery = from audit in _context.AuditLogs
                          where audit.Parameters.Any(p => p.Name == GlobalConstants.AuditParameters.ClientId && p.Value == clientId)
                          orderby audit.Timestamp descending
                          select audit;

        var lastEvents = await auditsQuery.Take(_lastEventsToTake).ToListAsync(cancellationToken);

        var items = lastEvents.Select(audit => new ClientAuditListOverviewResponse
        {
            Id = audit.Id,
            Event = audit.Event,
            Timestamp = audit.Timestamp,
            Description = audit.EventDescription,
            Parameters = audit.Parameters.Select(p => new AuditLogParameterDto { Value = p.Value, Name = p.Name }),
            InitiatedBy = new UserInfoDto
            {
                Id = audit.CreatedBy.Id, Type = audit.CreatedBy.Type, Username = audit.CreatedBy.Username
            }
        }).ToList();

        return Result.Ok(new InfoList<ClientAuditListOverviewResponse>(items));
    }
}