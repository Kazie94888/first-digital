using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Wallets;

internal sealed class WalletDetailsQueryHandler : IQueryHandler<WalletDetailsQuery, WalletDetailsResponse>
{
    private readonly ReadOnlyDataContext _context;

    public WalletDetailsQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<WalletDetailsResponse>> Handle(WalletDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var response = await (from w in _context.Wallets
                              where w.ClientId == request.ClientId && w.Id == request.WalletId
                              select new WalletDetailsResponse
                              {
                                  Id = w.Id,
                                  Address = w.WalletAccount,
                                  Alias = w.Alias,
                                  Status = w.VerificationStatus,
                                  Archived = w.Archived,
                                  ArchivedAt = w.ArchivedAt,
                                  CreatedAt = w.CreatedAt,
                                  CreatedBy = w.CreatedBy.Username,
                              }).FirstAsync(cancellationToken);

        return Result.Ok(response);
    }
}