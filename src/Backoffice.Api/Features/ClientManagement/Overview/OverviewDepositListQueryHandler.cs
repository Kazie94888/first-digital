using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Base;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Overview;

internal sealed class OverviewDepositListQueryHandler : IQueryHandler<OverviewDepositListQuery, InfoList<OverviewDepositListItem>>
{
    private readonly ReadOnlyDataContext _context;

    public OverviewDepositListQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<InfoList<OverviewDepositListItem>>> Handle(OverviewDepositListQuery request, CancellationToken cancellationToken)
    {
        var depositBankAccounts = await (from db in _context.DepositBanks
                                         select new OverviewDepositListItem
                                         {
                                             Id = db.Id,
                                             Iban = db.Iban,
                                             Name = db.Name,
                                             IsDefault = db.Default
                                         }).ToListAsync(cancellationToken);

        var infoList = new InfoList<OverviewDepositListItem>(depositBankAccounts);
        return Result.Ok(infoList);
    }
}