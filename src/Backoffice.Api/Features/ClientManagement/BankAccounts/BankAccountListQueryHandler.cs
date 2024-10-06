using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Base;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.BankAccounts;

internal sealed class BankAccountListQueryHandler : IQueryHandler<BankAccountListQuery, InfoList<BankAccountListResponse>>
{
    private readonly ReadOnlyDataContext _context;

    public BankAccountListQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }


    public async Task<Result<InfoList<BankAccountListResponse>>> Handle(BankAccountListQuery request, CancellationToken cancellationToken)
    {
        var bankAccounts = await (from b in _context.BankAccounts
                                  where b.ClientId == request.ClientId
                                  select new BankAccountListResponse
                                  {
                                      Id = b.Id,
                                      BankName = b.BankName,
                                      Beneficiary = b.Beneficiary,
                                      Iban = b.Iban,
                                      SwiftCode = b.SwiftCode,
                                      Alias = b.Alias,
                                      CreatedAt = b.CreatedAt,
                                      Status = b.VerificationStatus,
                                      Archived = b.Archived,
                                      ArchivedAt = b.ArchivedAt,
                                  }).ToListAsync(cancellationToken);

        var bankAccountsInfo = new InfoList<BankAccountListResponse>(bankAccounts);

        var activeCount = bankAccounts.Count(x => !x.Archived);
        var inActiveCount = bankAccounts.Count - activeCount;

        bankAccountsInfo.AddInfo("countActive", activeCount);
        bankAccountsInfo.AddInfo("countArchived", inActiveCount);

        return Result.Ok(bankAccountsInfo);
    }
}