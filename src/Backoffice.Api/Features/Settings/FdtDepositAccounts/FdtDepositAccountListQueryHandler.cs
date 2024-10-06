using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Base;

namespace SmartCoinOS.Backoffice.Api.Features.Settings.FdtDepositAccounts;

internal sealed class FdtDepositAccountListQueryHandler : IQueryHandler<FdtDepositAccountListQuery, InfoList<FdtDepositAccountListItem>>
{
    private readonly ReadOnlyDataContext _context;

    public FdtDepositAccountListQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<InfoList<FdtDepositAccountListItem>>> Handle(FdtDepositAccountListQuery request,
        CancellationToken cancellationToken)
    {
        var fdtDepositAccounts = await _context.FdtDepositAccounts
            .Select(acc => new FdtDepositAccountListItem
            {
                Id = acc.Id,
                AccountName = acc.AccountName,
                AccountNumber = acc.AccountNumber,
                Archived = acc.Archived,
                ArchivedAt = acc.ArchivedAt,
                CreatedAt = acc.CreatedAt,
            })
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        var activeCount = fdtDepositAccounts.Count(x => !x.Archived);
        var inActiveCount = fdtDepositAccounts.Count - activeCount;

        var list = new InfoList<FdtDepositAccountListItem>(fdtDepositAccounts).Sorted(x => x.CreatedAt, false);
        list.AddInfo("countActive", activeCount);
        list.AddInfo("countArchived", inActiveCount);

        return Result.Ok(list);
    }
}