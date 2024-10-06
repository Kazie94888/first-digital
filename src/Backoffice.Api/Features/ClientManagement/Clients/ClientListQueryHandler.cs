using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Domain.Orders;
using SmartCoinOS.Extensions.Fiql;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Clients;

internal sealed class ClientListQueryHandler : IQueryHandler<ClientListQuery, InfoPagedList<ClientListResponse>>
{
    private readonly ReadOnlyDataContext _context;

    public ClientListQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<InfoPagedList<ClientListResponse>>> Handle(ClientListQuery request,
        CancellationToken cancellationToken)
    {
        var activeOrderStatuses = new OrderStatus[]
        {
            OrderStatus.Initiated,
            OrderStatus.InProgress
        };
        var clientsQuery = (from c in _context.Clients
            join ep in _context.EntityParticulars on c.Id equals ep.ClientId
            where ep.Archived == false
            select new ClientListResponse
            {
                Id = c.Id,
                ClientName = ep.LegalEntityName,
                CreatedAt = c.CreatedAt,
                Status = c.Status,
                Orders = new ClientListOrders
                {
                    Active =
                        _context.Orders.Count(o => o.ClientId == c.Id && activeOrderStatuses.Contains(o.Status)),
                    Total = _context.Orders.Count(o => o.ClientId == c.Id)
                },
                Minted = new MoneyDto
                {
                    Amount = _context.Orders
                        .Where(o => o.ClientId == c.Id
                                    && o.Type == OrderType.Mint
                                    && o.OrderedAmount.Currency == GlobalConstants.Currency.Fdusd)
                        .Sum(a => a.OrderedAmount.Amount),
                    AssetSymbol = GlobalConstants.Currency.Fdusd
                },
                Redeemed = new MoneyDto
                {
                    Amount = _context.Orders
                        .Where(o => o.ClientId == c.Id
                                    && o.Type == OrderType.Redeem
                                    && o.OrderedAmount.Currency == GlobalConstants.Currency.Fdusd)
                        .Sum(a => a.OrderedAmount.Amount),
                    AssetSymbol = GlobalConstants.Currency.Fdusd
                }
            }).Filter(request);

        var totalCount = await clientsQuery.CountAsync(cancellationToken);
        var clients = await clientsQuery.SortAndPage(request).ToListAsync(cancellationToken);

        return Result.Ok(new InfoPagedList<ClientListResponse>(clients, totalCount, request.Page, request.Take));
    }
}
