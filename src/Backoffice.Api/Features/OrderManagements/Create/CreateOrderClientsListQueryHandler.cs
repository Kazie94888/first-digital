using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Create;

internal sealed class
    CreateOrderClientsListQueryHandler : IQueryHandler<CreateOrderClientsListQuery,
    InfoList<CreateOrderClientsResponse>>
{
    private readonly ReadOnlyDataContext _context;

    public CreateOrderClientsListQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<InfoList<CreateOrderClientsResponse>>> Handle(CreateOrderClientsListQuery request,
        CancellationToken cancellationToken)
    {
        var clients =
            await (from c in _context.Clients
                join ep in _context.EntityParticulars on c.Id equals ep.ClientId
                where c.Status == ClientStatus.Active
                      && !ep.Archived
                select new CreateOrderClientsResponse
                {
                    Id = c.Id,
                    Name = ep.LegalEntityName
                }).ToListAsync(cancellationToken);

        var infoList = new InfoList<CreateOrderClientsResponse>(clients);
        return Result.Ok(infoList);
    }
}
