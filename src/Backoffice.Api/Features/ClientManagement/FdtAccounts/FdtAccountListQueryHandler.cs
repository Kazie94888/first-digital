using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Base;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.FdtAccounts;

internal sealed class FdtAccountListQueryHandler : IQueryHandler<FdtAccountListQuery, InfoList<FdtAccountListResponse>>
{
    private readonly ReadOnlyDataContext _context;

    public FdtAccountListQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<InfoList<FdtAccountListResponse>>> Handle(FdtAccountListQuery request, CancellationToken cancellationToken)
    {
        var fdtAccounts = await (from a in _context.FdtAccounts
                                 where a.ClientId == request.ClientId
                                 select new FdtAccountListResponse
                                 {
                                     Id = a.Id,
                                     ClientName = a.ClientName,
                                     AccountNumber = a.AccountNumber,
                                     Alias = a.Alias,
                                     CreatedAt = a.CreatedAt,
                                     Status = a.VerificationStatus,
                                     Archived = a.Archived,
                                     ArchivedAt = a.ArchivedAt
                                 }).ToListAsync(cancellationToken);

        var countActive = fdtAccounts.Count(x => !x.Archived);
        var countArchived = fdtAccounts.Count - countActive;

        var responseResult = new InfoList<FdtAccountListResponse>(fdtAccounts);
        responseResult.AddInfo("countActive", countActive);
        responseResult.AddInfo("countArchived", countArchived);

        return Result.Ok(responseResult);
    }
}