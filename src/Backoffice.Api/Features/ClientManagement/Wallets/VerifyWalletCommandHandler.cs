using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Wallets;

internal sealed class VerifyWalletCommandHandler : ICommandHandler<VerifyWalletCommand, EntityId>
{
    private readonly DataContext _context;

    public VerifyWalletCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<EntityId>> Handle(VerifyWalletCommand request, CancellationToken cancellationToken)
    {
        var client = await _context.Clients.FirstAsync(x => x.Id == request.ClientId, cancellationToken);

        var verifyWalletResult = client.VerifyWallet(request.WalletId, request.UserInfo);
        if (verifyWalletResult.IsFailed)
            return verifyWalletResult;

        return Result.Ok(new EntityId(request.WalletId.Value));
    }
}