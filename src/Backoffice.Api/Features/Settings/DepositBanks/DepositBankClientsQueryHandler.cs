using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Extensions.Fiql;

namespace SmartCoinOS.Backoffice.Api.Features.Settings.DepositBanks;

internal sealed class DepositBankClientsQueryHandler : IQueryHandler<DepositBankClientsQuery, InfoPagedList<ClientsDepositBank>>
{
    private readonly ReadOnlyDataContext _context;

    public DepositBankClientsQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<InfoPagedList<ClientsDepositBank>>> Handle(DepositBankClientsQuery request, CancellationToken cancellationToken)
    {
        var clientsQuery = (from c in _context.Clients
                            join ep in _context.EntityParticulars on c.Id equals ep.ClientId
                            where !ep.Archived
                                    && c.DepositBankId == request.Id
                            select new ClientsDepositBank
                            {
                                Id = c.Id,
                                LegalEntityName = ep.LegalEntityName,
                                CreatedAt = c.CreatedAt,
                                Orders = _context.Orders.Count(o => o.ClientId == c.Id)
                            }).Filter(request);

        var totalCount = await clientsQuery.CountAsync(cancellationToken);
        var clients = await clientsQuery.SortAndPage(request).ToListAsync(cancellationToken);

        var pagedResult = new InfoPagedList<ClientsDepositBank>(clients, totalCount, request.Page, request.Take);

        return Result.Ok(pagedResult);
    }
}