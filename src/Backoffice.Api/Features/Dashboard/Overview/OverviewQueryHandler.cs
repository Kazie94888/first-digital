using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Application.Enums;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Domain.Orders;

namespace SmartCoinOS.Backoffice.Api.Features.Dashboard.Overview;

internal class OverviewQueryHandler : IQueryHandler<OverviewQuery, OverviewResponse>
{
    private readonly ReadOnlyDataContext _context;

    public OverviewQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<OverviewResponse>> Handle(OverviewQuery request, CancellationToken cancellationToken)
    {
        var filterFromDate = request.GetFromDate();

        var totals = await GetOrderTotalsAsync(filterFromDate, cancellationToken);
        var ordersInProgress = await GetOrdersInProgressAsync(cancellationToken);
        var applicationsInProgress = await ApplicationsInProgressAsync(cancellationToken);

        var response = new OverviewResponse
        {
            Totals = totals,
            OrdersInProgress = ordersInProgress,
            ApplicationInProgress = applicationsInProgress
        };

        return Result.Ok(response);
    }

    private async Task<List<OverviewAggregateResponse>> GetOrderTotalsAsync(DateTimeOffset? fromDate,
        CancellationToken cancellationToken)
    {
        var filteredStatuses = new[]
        {
            OrderStatus.InProgress,
            OrderStatus.Completed
        };

        var query = _context.Orders.Where(x => filteredStatuses.Contains(x.Status) && x.ActualAmount != null);

        if (fromDate.HasValue)
            query = query.Where(x => x.CreatedAt > fromDate);

        var aggTotals = await
            (from o in query
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
        var aggregatedResponse = new List<OverviewAggregateResponse>();

        foreach (var coin in stablecoins)
        {
            var coinAgg = aggTotals.Where(x => x.Stablecoin == coin).ToList();

            aggregatedResponse.Add(new OverviewAggregateResponse
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

        return aggregatedResponse;
    }

    private async Task<InfoList<OverviewOrderInProgress>> GetOrdersInProgressAsync(
        CancellationToken cancellationToken)
    {
        var ordersQuery = (
            from order in _context.Orders
            join client in _context.Clients on order.ClientId equals client.Id
            join ep in _context.EntityParticulars on client.Id equals ep.ClientId
            where order.Status == OrderStatus.InProgress
                  && !ep.Archived
            select new OverviewOrderInProgress
            {
                Id = order.Id,
                PrettyId = order.OrderNumber.Value,
                OrderType = order.Type,
                CreatedAt = order.CreatedAt,
                ClientName = ep.LegalEntityName,
                Progress = order.Status,
                ProcessingStatus = order.ProcessingStatus,
                AmountOrdered = new MoneyDto
                {
                    Amount = order.OrderedAmount.Amount,
                    AssetSymbol = order.OrderedAmount.Currency
                }
            }
        );

        var ordersInProgress = await ordersQuery.ToListAsync(cancellationToken);
        var ordersInfoList = new InfoList<OverviewOrderInProgress>(ordersInProgress);
        ordersInfoList.AddInfo("totalCount", ordersInProgress.Count);

        return ordersInfoList;
    }

    private async Task<InfoList<OverviewApplicationsInProgress>> ApplicationsInProgressAsync(
        CancellationToken cancellationToken)
    {
        var filterStatuses = new[]
        {
            ApplicationStatus.InReview,
            ApplicationStatus.AdditionalInformationRequired
        };

        var applications = await
            (from a in _context.Applications
                where filterStatuses.Contains(a.Status)
                select new OverviewApplicationsInProgress
                {
                    Id = a.Id,
                    LegalEntityName = a.LegalEntityName,
                    CreatedAt = a.CreatedAt,
                    CreatedBy = a.CreatedBy,
                    PrettyId = a.ApplicationNumber,
                    Status = a.Status
                }).ToListAsync(cancellationToken);

        var applicationList =
            new InfoList<OverviewApplicationsInProgress>(applications);
        applicationList.AddInfo("totalCount", applications.Count);

        return applicationList;
    }
}
