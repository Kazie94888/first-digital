using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.ClientPortal.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Persistence;

namespace SmartCoinOS.ClientPortal.Api.Features.CompanyInformation.FdtAccounts;

internal sealed class FdtAccountDetailsQueryHandler : IQueryHandler<FdtAccountDetailsQuery, FdtAccountDetailsResponse>
{
    private readonly ReadOnlyDataContext _context;
    private const int _recentOrderCount = 5;

    public FdtAccountDetailsQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<FdtAccountDetailsResponse>> Handle(FdtAccountDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var clientId = request.UserInfo.ClientId;

        var fdtAccountDetails = new FdtAccountDetailsResponse
        {
            FdtAccount = await GetFdtAccountDetailsAsync(clientId, request.FdtAccountId, cancellationToken),
            RelatedOrders = await GetRelatedOrdersAsync(clientId, request.FdtAccountId, cancellationToken)
        };

        return Result.Ok(fdtAccountDetails);
    }

    private async Task<FdtAccountDetails> GetFdtAccountDetailsAsync(ClientId clientId, FdtAccountId fdtAccountId,
        CancellationToken cancellationToken)
    {
        var fdtAccountDetails =
            await (from f in _context.FdtAccounts
                where f.ClientId == clientId && f.Id == fdtAccountId
                select new FdtAccountDetails
                {
                    Id = f.Id,
                    Account = new FdtAccountDto()
                    {
                        Name = f.ClientName,
                        Number = f.AccountNumber.Value
                    },
                    Archived = f.Archived,
                    Status = f.VerificationStatus,
                    Alias = f.Alias,
                    CreatedAt = f.CreatedAt,
                    CreatedBy = new UserInfoDto()
                    {
                        Id = f.CreatedBy.Id,
                        Username = f.CreatedBy.Username,
                        ClientId = clientId
                    }
                }).FirstAsync(cancellationToken);

        return fdtAccountDetails;
    }

    private async Task<List<RelatedOrdersDto>> GetRelatedOrdersAsync(ClientId clientId, FdtAccountId fdtAccountId,
        CancellationToken cancellationToken)
    {
        var orders = await _context.Orders.Where(x => x.ClientId == clientId && x.FdtAccountId == fdtAccountId)
            .Select(x => new RelatedOrdersDto
            {
                Id = x.Id,
                Type = x.Type,
                PrettyId = x.OrderNumber.Value,
                CreatedAt = x.CreatedAt,
                Status = x.Status,
                Amount = x.OrderedAmount
            }).OrderByDescending(x => x.CreatedAt)
            .Take(_recentOrderCount)
            .ToListAsync(cancellationToken);

        return orders;
    }
}
