using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Base;

namespace SmartCoinOS.Backoffice.Api.Features.Settings.DepositBanks;

internal sealed class DepositBankListQueryHandler : IQueryHandler<DepositBankListQuery, InfoList<DepositBankListItem>>
{
    private readonly ReadOnlyDataContext _context;

    public DepositBankListQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<InfoList<DepositBankListItem>>> Handle(DepositBankListQuery request, CancellationToken cancellationToken)
    {
        var dbDepositBanks = await _context.DepositBanks
                                            .Include(x => x.Address)
                                            .OrderByDescending(x => x.CreatedAt)
                                            .ToListAsync(cancellationToken);

        var depositBanks = (from b in dbDepositBanks
                            select new DepositBankListItem
                            {
                                Id = b.Id,
                                Name = b.Name,
                                Beneficiary = b.Beneficiary,
                                Swift = b.Swift,
                                Iban = b.Iban,
                                Address = b.Address.GetFullAddress(),
                                CreatedAt = b.CreatedAt,
                                Default = b.Default,
                                Archived = b.Archived
                            }).ToList();

        var activeCount = depositBanks.Count(x => !x.Archived);
        var inActiveCount = depositBanks.Count - activeCount;

        var bankAccountsInfo = new InfoList<DepositBankListItem>(depositBanks).Sorted(x => x.CreatedAt, false);
        bankAccountsInfo.AddInfo("countActive", activeCount);
        bankAccountsInfo.AddInfo("countArchived", inActiveCount);

        return Result.Ok(bankAccountsInfo);
    }
}