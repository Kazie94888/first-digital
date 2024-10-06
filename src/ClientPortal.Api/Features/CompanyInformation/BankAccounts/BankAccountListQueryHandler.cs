using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Persistence;

namespace SmartCoinOS.ClientPortal.Api.Features.CompanyInformation.BankAccounts;

internal sealed class
    BankAccountListQueryHandler : IQueryHandler<BankAccountListQuery, InfoList<BankAccountListResponse>>
{
    private readonly ReadOnlyDataContext _context;

    public BankAccountListQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<InfoList<BankAccountListResponse>>> Handle(BankAccountListQuery request,
        CancellationToken cancellationToken)
    {
        var clientId = request.UserInfo.ClientId;

        var bankAccounts = await (from b in _context.BankAccounts
            let orders = _context.Orders.Count(x => x.BankAccountId == b.Id)
            where b.ClientId == clientId
            select new BankAccountListResponse
            {
                Id = b.Id,
                Alias = b.Alias,
                Iban = b.Iban,
                BankName = b.BankName,
                Status = b.VerificationStatus,
                Beneficiary = b.Beneficiary,
                SwiftCode = b.SwiftCode,
                CreatedAt = b.CreatedAt,
                Archived = b.Archived,
                ArchivedAt = b.ArchivedAt,
                OrderCount = orders
            }).ToListAsync(cancellationToken);

        var bankAccountsInfo = new InfoList<BankAccountListResponse>(bankAccounts);
        return Result.Ok(bankAccountsInfo);
    }
}
