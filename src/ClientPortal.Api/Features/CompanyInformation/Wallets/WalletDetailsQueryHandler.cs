using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.ClientPortal.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Persistence;

namespace SmartCoinOS.ClientPortal.Api.Features.CompanyInformation.Wallets;

internal sealed class WalletDetailsQueryHandler : IQueryHandler<WalletDetailsQuery, WalletDetailsResponse>
{
    private readonly ReadOnlyDataContext _context;
    private const int _recentOrderCount = 5;

    public WalletDetailsQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<WalletDetailsResponse>> Handle(WalletDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var clientId = request.UserInfo.ClientId;

        var walletDetails = new WalletDetailsResponse
        {
            Wallet = await GetWalletDetailsAsync(clientId, request.WalletId, cancellationToken),
            RelatedOrders = await GetRelatedOrdersAsync(clientId, request.WalletId, cancellationToken)
        };

        return Result.Ok(walletDetails);
    }

    private async Task<WalletDetails> GetWalletDetailsAsync(ClientId clientId, WalletId walletId,
        CancellationToken cancellationToken)
    {
        var walletDetails = await (from w in _context.Wallets
            where w.ClientId == clientId && w.Id == walletId
            select new WalletDetails
            {
                Id = w.Id,
                Address = new BlockchainAddressDto()
                {
                    Address = w.WalletAccount.Address,
                    Network = w.WalletAccount.Network
                },
                Alias = w.Alias,
                Status = w.VerificationStatus,
                Archived = w.Archived,
                CreatedAt = w.CreatedAt,
                CreatedBy = new UserInfoDto()
                {
                    Id = w.CreatedBy.Id,
                    Username = w.CreatedBy.Username,
                    ClientId = clientId
                }
            }).FirstAsync(cancellationToken);

        walletDetails.Totals = await (from o in _context.Orders
                where o.ClientId == clientId && o.WalletId == walletId
                group o by new
                {
                    o.Type,
                    o.OrderedAmount.Currency
                } into grp
                select new WalletTotals
                {
                    Type = grp.Key.Type,
                    Amount = new MoneyDto()
                    {
                        Amount = grp.Sum(g => g.OrderedAmount.Amount),
                        AssetSymbol = grp.Key.Currency
                    }
                })
            .ToListAsync(cancellationToken);

        return walletDetails;
    }

    private async Task<List<RelatedOrdersDto>> GetRelatedOrdersAsync(ClientId clientId, WalletId walletId,
        CancellationToken cancellationToken)
    {
        var orders = await (from o in _context.Orders
                where o.ClientId == clientId && o.WalletId == walletId
                select new RelatedOrdersDto()
                {
                    Id = o.Id,
                    Type = o.Type,
                    PrettyId = o.OrderNumber.Value,
                    CreatedAt = o.CreatedAt,
                    Status = o.Status,
                    Amount = o.OrderedAmount
                }).OrderByDescending(x => x.CreatedAt)
            .Take(_recentOrderCount)
            .ToListAsync(cancellationToken);

        return orders;
    }
}
