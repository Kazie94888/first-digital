using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Base;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Overview;

internal sealed class ClientOrderOverviewListQueryHandler : IQueryHandler<ClientOrderOverviewListQuery, InfoList<ClientOrderOverviewListResponse>>
{
    private readonly ReadOnlyDataContext _context;

    public ClientOrderOverviewListQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<InfoList<ClientOrderOverviewListResponse>>> Handle(ClientOrderOverviewListQuery request, CancellationToken cancellationToken)
    {
        const int takeLast = 4;

        var query = from o in _context.Orders
                    where o.ClientId == request.ClientId
                    orderby o.CreatedAt descending
                    select new ClientOrderOverviewListResponse
                    {
                        Id = o.Id,
                        PrettyId = o.OrderNumber.Value,
                        CreatedAt = o.CreatedAt,
                        MintedBurned = o.ActualAmount == null ? null : new MoneyDto
                        {
                            Amount = o.ActualAmount.Amount,
                            AssetSymbol = o.ActualAmount.Currency
                        },
                        Ordered = new MoneyDto
                        {
                            Amount = o.OrderedAmount.Amount,
                            AssetSymbol = o.OrderedAmount.Currency
                        },
                        OrderType = o.Type,
                        ProcessingStatus = o.ProcessingStatus,
                        Status = o.Status
                    };

        var result = await query.Take(takeLast).ToListAsync(cancellationToken);

        return Result.Ok(new InfoList<ClientOrderOverviewListResponse>(result).Sorted(x => x.CreatedAt, asc: false));
    }
}
