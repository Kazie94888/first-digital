using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Extensions.Fiql;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Wallets;

internal sealed class WalletOrdersQueryHandler
    : IQueryHandler<WalletOrdersQuery, InfoPagedList<WalletOrdersResponse>>
{
    private readonly ReadOnlyDataContext _context;

    public WalletOrdersQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<InfoPagedList<WalletOrdersResponse>>> Handle(WalletOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var ordersQuery = (
            from order in _context.Orders
            join client in _context.Clients on order.ClientId equals client.Id
            join ep in _context.EntityParticulars on client.Id equals ep.ClientId
            join wallet in _context.Wallets on order.WalletId equals wallet.Id
            where order.ClientId == request.ClientId
                  && order.WalletId == request.WalletId
                  && !ep.Archived
            select new WalletOrdersResponse
            {
                Id = order.Id,
                PrettyId = order.OrderNumber.Value,
                OrderType = order.Type,
                CreatedAt = order.CreatedAt,
                ClientName = ep.LegalEntityName,
                Progress = order.Status,
                SubStatus = order.ProcessingStatus,
                Amount = order.ActualAmount == null
                    ? null
                    : new MoneyDto
                    {
                        Amount = order.ActualAmount.Amount,
                        AssetSymbol = order.ActualAmount.Currency
                    },
                AmountOrdered = new MoneyDto
                {
                    Amount = order.OrderedAmount.Amount,
                    AssetSymbol = order.OrderedAmount.Currency
                }
            }
        ).Filter(request);

        var totalCount = await ordersQuery.CountAsync(cancellationToken);
        var orders = await ordersQuery.SortAndPage(request).ToListAsync(cancellationToken);

        return Result.Ok(new InfoPagedList<WalletOrdersResponse>(orders, totalCount, request.Page, request.Take)
            .Sorted(x => x.CreatedAt, asc: false));
    }
}
