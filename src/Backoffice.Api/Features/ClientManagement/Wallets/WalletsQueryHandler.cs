using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Base;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Wallets;

internal sealed class WalletsQueryHandler : IQueryHandler<WalletsQuery, InfoList<WalletsResponse>>
{
    private readonly ReadOnlyDataContext _context;

    public WalletsQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<InfoList<WalletsResponse>>> Handle(WalletsQuery request, CancellationToken cancellationToken)
    {
        var wallets = await (from w in _context.Wallets
                             where w.ClientId == request.ClientId
                             select new WalletsResponse
                             {
                                 Id = w.Id,
                                 Address = w.WalletAccount.Address,
                                 Network = w.WalletAccount.Network,
                                 Alias = w.Alias,
                                 Status = w.VerificationStatus,
                                 CreatedAt = w.CreatedAt,
                                 OrderCount = w.OrdersCount,
                                 Archived = w.Archived,
                                 ArchivedAt = w.ArchivedAt
                             }).ToListAsync(cancellationToken);

        var countActive = wallets.Count(x => !x.Archived);
        var countArchived = wallets.Count - countActive;

        var responseResult = new InfoList<WalletsResponse>(wallets);
        responseResult.AddInfo("countActive", countActive);
        responseResult.AddInfo("countArchived", countArchived);

        return Result.Ok(responseResult);
    }
}