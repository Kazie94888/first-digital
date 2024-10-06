using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.FdtAccounts;

internal sealed class FdtAccountDetailsQueryHandler : IQueryHandler<FdtAccountDetailsQuery, FdtAccountDetailsResponse>
{
    private readonly ReadOnlyDataContext _context;

    public FdtAccountDetailsQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<FdtAccountDetailsResponse>> Handle(FdtAccountDetailsQuery request, CancellationToken cancellationToken)
    {
        var response = await (from a in _context.FdtAccounts
                              where a.ClientId == request.ClientId && a.Id == request.FdtAccountId
                              select new FdtAccountDetailsResponse
                              {
                                  Id = a.Id,
                                  ClientName = a.ClientName,
                                  AccountNumber = a.AccountNumber,
                                  Alias = a.Alias,
                                  CreatedAt = a.CreatedAt,
                                  CreatedBy = a.CreatedBy,
                                  Archived = a.Archived,
                                  ArchivedAt = a.ArchivedAt,
                                  Status = a.VerificationStatus
                              }).FirstAsync(cancellationToken);

        return Result.Ok(response);
    }
}