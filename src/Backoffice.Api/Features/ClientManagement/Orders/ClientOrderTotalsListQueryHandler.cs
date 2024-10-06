using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Domain.Orders;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Orders;

internal sealed class
    ClientOrderTotalsListQueryHandler : IQueryHandler<ClientOrderTotalsListQuery, List<ClientOrderTotalsListResponse>>
{
    private readonly ReadOnlyDataContext _context;

    public ClientOrderTotalsListQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<List<ClientOrderTotalsListResponse>>> Handle(ClientOrderTotalsListQuery request,
        CancellationToken cancellationToken)
    {
        var filteredStatuses = new[]
        {
            OrderStatus.InProgress,
            OrderStatus.Completed
        };

        var aggTotals = await (from o in _context.Orders
            where o.ClientId == request.ClientId
                  && filteredStatuses.Contains(o.Status)
                  && o.ActualAmount != null
            group o by new
            {
                o.Type,
                o.Status,
                o.ActualAmount!.Currency
            } into aggResult
            select new
            {
                OrderType = aggResult.Key.Type,
                OrderStatus = aggResult.Key.Status,
                Stablecoin = aggResult.Key.Currency,
                Total = aggResult.Sum(x => x.ActualAmount!.Amount)
            }).ToListAsync(cancellationToken);

        var stablecoins = GlobalConstants.Currency.SupportedStablecoins();

        var aggregatedResponse = new List<ClientOrderTotalsListResponse>();

        foreach (var coin in stablecoins)
        {
            var coinAgg = aggTotals.Where(x => x.Stablecoin == coin).ToList();

            aggregatedResponse.Add(new ClientOrderTotalsListResponse
            {
                MintInProgress = new MoneyDto
                {
                    Amount = coinAgg
                        .Where(x => x.OrderType == OrderType.Mint && x.OrderStatus == OrderStatus.InProgress)
                        .Select(x => x.Total)
                        .FirstOrDefault(),
                    AssetSymbol = coin
                },
                MintCompleted = new MoneyDto
                {
                    Amount = coinAgg.Where(x => x.OrderType == OrderType.Mint && x.OrderStatus == OrderStatus.Completed)
                        .Select(x => x.Total)
                        .FirstOrDefault(),
                    AssetSymbol = coin
                },
                RedeemInProgress = new MoneyDto
                {
                    Amount = coinAgg.Where(x =>
                            x.OrderType == OrderType.Redeem && x.OrderStatus == OrderStatus.InProgress)
                        .Select(x => x.Total)
                        .FirstOrDefault(),
                    AssetSymbol = coin
                },
                RedeemCompleted = new MoneyDto
                {
                    Amount = coinAgg.Where(x =>
                            x.OrderType == OrderType.Redeem && x.OrderStatus == OrderStatus.Completed)
                        .Select(x => x.Total)
                        .FirstOrDefault(),
                    AssetSymbol = coin
                }
            });
        }

        return Result.Ok(aggregatedResponse);
    }
}
