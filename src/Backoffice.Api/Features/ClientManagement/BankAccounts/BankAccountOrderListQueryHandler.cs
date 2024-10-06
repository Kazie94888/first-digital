using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Extensions.Fiql;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.BankAccounts;

internal sealed class BankAccountOrderListQueryHandler
    : IQueryHandler<BankAccountOrderListQuery, InfoPagedList<BankAccountOrderListResponse>>
{
    private readonly ReadOnlyDataContext _context;

    public BankAccountOrderListQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<InfoPagedList<BankAccountOrderListResponse>>> Handle(BankAccountOrderListQuery request,
        CancellationToken cancellationToken)
    {
        var query = from order in _context.Orders
                    where order.ClientId == request.ClientId && order.BankAccountId == request.BankAccountId
                    select new BankAccountOrderListResponse
                    {
                        Id = order.Id,
                        PrettyId = order.OrderNumber.Value,
                        OrderType = order.Type,
                        Status = order.Status,
                        ProcessingStatus = order.ProcessingStatus,
                        MintedBurned = order.ActualAmount == null ? null : new MoneyDto
                        {
                            Amount = order.ActualAmount.Amount,
                            AssetSymbol = order.ActualAmount.Currency
                        },
                        Ordered = new MoneyDto
                        {
                            Amount = order.OrderedAmount.Amount,
                            AssetSymbol = order.OrderedAmount.Currency
                        },
                        CreatedAt = order.CreatedAt
                    };

        var totalCount = await query.CountAsync(cancellationToken);
        var accountOrders = await query.SortAndPage(request).ToListAsync(cancellationToken);

        return Result.Ok(new InfoPagedList<BankAccountOrderListResponse>(accountOrders, totalCount, request.Page, request.Take));
    }
}