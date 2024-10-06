using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Base;

namespace SmartCoinOS.Backoffice.Api.Features.Settings.DepositWallets;

internal sealed class DepositWalletListQueryHandler : IQueryHandler<DepositWalletListQuery, InfoList<DepositWalletListItem>>
{
    private readonly ReadOnlyDataContext _context;

    public DepositWalletListQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<InfoList<DepositWalletListItem>>> Handle(DepositWalletListQuery request, CancellationToken cancellationToken)
    {
        var depositWallets = await (from dw in _context.DepositWallets
                                    select new DepositWalletListItem
                                    {
                                        Id = dw.Id,
                                        Address = dw.Account,
                                        CreatedAt = dw.CreatedAt
                                    }).OrderByDescending(x => x.CreatedAt)
                              .ToListAsync(cancellationToken);

        var infoList = new InfoList<DepositWalletListItem>(depositWallets)
                                .Sorted(x => x.CreatedAt, false);

        return Result.Ok(infoList);
    }
}