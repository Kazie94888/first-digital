using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.BankAccounts;

internal sealed class BankAccountDetailsQueryHandler : IQueryHandler<BankAccountDetailsQuery,
    BankAccountDetailsResponse>
{
    private readonly ReadOnlyDataContext _context;

    public BankAccountDetailsQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<BankAccountDetailsResponse>> Handle(BankAccountDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var response = await (from b in _context.BankAccounts
            where b.ClientId == request.ClientId && b.Id == request.BankAccountId
            select new BankAccountDetailsResponse
            {
                Id = b.Id,
                Alias = b.Alias,
                BankName = b.BankName,
                Status = b.VerificationStatus,
                Beneficiary = b.Beneficiary,
                SwiftCode = b.SwiftCode,
                Iban = b.Iban,
                BankAddress = b.Address,
                CreatedAt = b.CreatedAt,
                CreatedBy = b.CreatedBy,
                Archived = b.Archived,
                ArchivedAt = b.ArchivedAt,
                OwnBankId = b.SmartTrustBank.OwnBankId,
                ThirdPartyBankId = b.SmartTrustBank.ThirdPartyBankId
            }).FirstAsync(cancellationToken);

        return Result.Ok(response);
    }
}
