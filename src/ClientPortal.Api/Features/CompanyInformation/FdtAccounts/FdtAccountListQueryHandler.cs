using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.ClientPortal.Api.Base.Dto;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Persistence;

namespace SmartCoinOS.ClientPortal.Api.Features.CompanyInformation.FdtAccounts;

internal sealed class FdtAccountListQueryHandler : IQueryHandler<FdtAccountListQuery, InfoList<FdtAccountListResponse>>
{
    private readonly ReadOnlyDataContext _context;

    public FdtAccountListQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<InfoList<FdtAccountListResponse>>> Handle(FdtAccountListQuery request,
        CancellationToken cancellationToken)
    {
        var clientId = request.UserInfo.ClientId;

        var fdtAccounts = await (from f in _context.FdtAccounts
            where f.ClientId == clientId
            select new FdtAccountListResponse()
            {
                Id = f.Id,
                Account = new FdtAccountDto()
                {
                    Name = f.ClientName,
                    Number = f.AccountNumber.Value
                },
                Status = f.VerificationStatus,
                Archived = f.Archived,
                Alias = f.Alias
            }).ToListAsync(cancellationToken);

        var fdtAccountList = new InfoList<FdtAccountListResponse>(fdtAccounts);
        return Result.Ok(fdtAccountList);
    }
}