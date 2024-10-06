using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.ClientPortal.Api.Base.Dto;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Persistence;

namespace SmartCoinOS.ClientPortal.Api.Features.CompanyInformation.Wallets;

internal sealed class WalletListQueryHandler : IQueryHandler<WalletListQuery, InfoList<WalletListResponse>>
{
    private readonly ReadOnlyDataContext _context;

    public WalletListQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<InfoList<WalletListResponse>>> Handle(WalletListQuery request,
        CancellationToken cancellationToken)
    {
        var clientId = request.UserInfo.ClientId;

        var wallets = await (from w in _context.Wallets
            let orders = _context.Orders.Count(o => o.WalletId == w.Id)
            where w.ClientId == clientId
            select new WalletListResponse()
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
                OrderCount = orders
            }).ToListAsync(cancellationToken);

        var responseResult = new InfoList<WalletListResponse>(wallets);
        return Result.Ok(responseResult);
    }
}
