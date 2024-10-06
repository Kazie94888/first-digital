using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;

namespace SmartCoinOS.Backoffice.Api.Features.Settings.FdtDepositAccounts;

internal sealed class FdtDepositAccountDetailsQueryHandler : IQueryHandler<FdtDepositAccountDetailsQuery, FdtDepositAccountDetailsResponse>
{
    private readonly ReadOnlyDataContext _context;

    public FdtDepositAccountDetailsQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<FdtDepositAccountDetailsResponse>> Handle(FdtDepositAccountDetailsQuery request, CancellationToken cancellationToken)
    {
        var detailsResponse = await _context.FdtDepositAccounts
            .Where(acc => acc.Id == request.Id)
            .Select(acc => new FdtDepositAccountDetailsResponse
            {
                Id = acc.Id,
                AccountName = acc.AccountName,
                AccountNumber = acc.AccountNumber,
                CreatedAt = acc.CreatedAt,
                Archived = acc.Archived,
                ArchivedAt = acc.ArchivedAt,
                CreatedBy = acc.CreatedBy
            })
            .FirstAsync(cancellationToken);

        return Result.Ok(detailsResponse);
    }
}