using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Wallets;

internal sealed class DeclineWalletCommandHandler : ICommandHandler<DeclineWalletCommand, EntityId>
{
    private readonly DataContext _context;

    public DeclineWalletCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<EntityId>> Handle(DeclineWalletCommand request, CancellationToken cancellationToken)
    {
        var client = await _context.Clients.FirstAsync(x => x.Id == request.ClientId, cancellationToken);

        var declineWalletResult = client.DeclineWallet(request.WalletId, request.UserInfo);
        if (declineWalletResult.IsFailed)
            return declineWalletResult;

        return Result.Ok(new EntityId(request.WalletId.Value));
    }
}