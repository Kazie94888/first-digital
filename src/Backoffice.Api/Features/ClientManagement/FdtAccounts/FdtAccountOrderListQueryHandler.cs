using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Extensions.Fiql;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.FdtAccounts;

internal sealed class
    FdtAccountOrderListQueryHandler : IQueryHandler<FdtAccountOrderListQuery,
    InfoPagedList<FdtAccountOrderListResponse>>
{
    private readonly ReadOnlyDataContext _context;

    public FdtAccountOrderListQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<InfoPagedList<FdtAccountOrderListResponse>>> Handle(FdtAccountOrderListQuery request,
        CancellationToken cancellationToken)
    {
        var ordersQuery = from order in _context.Orders
            join client in _context.Clients on order.ClientId equals client.Id
            join ep in _context.EntityParticulars on client.Id equals ep.ClientId
            join fdtAccount in _context.FdtAccounts on order.FdtAccountId equals fdtAccount.Id
            where order.ClientId == request.ClientId
                  && order.FdtAccountId == request.FdtAccountId
                  && !ep.Archived
            select new FdtAccountOrderListResponse
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
            };

        var totalCount = await ordersQuery.CountAsync(cancellationToken);
        var orders = await ordersQuery.SortAndPage(request).ToListAsync(cancellationToken);

        return Result.Ok(
            new InfoPagedList<FdtAccountOrderListResponse>(orders, totalCount, request.Page, request.Take));
    }
}
