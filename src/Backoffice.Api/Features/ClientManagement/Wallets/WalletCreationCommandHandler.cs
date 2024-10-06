using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Wallets;

public sealed class WalletCreationCommandHandler : ICommandHandler<WalletCreationCommand, EntityId>
{
    private readonly DataContext _context;

    public WalletCreationCommandHandler(DataContext dataContext)
    {
        _context = dataContext;
    }

    public async Task<Result<EntityId>> Handle(WalletCreationCommand request, CancellationToken cancellationToken)
    {
        var client = await _context.Clients.Where(x => x.Id == request.ClientId).FirstAsync(cancellationToken);

        var addResult = client.AddWallet(request.Wallet.Address, request.Wallet.Network, request.Wallet.Alias, request.UserInfo);

        if (addResult.IsFailed)
            return Result.Fail(addResult.Errors);

        var wallet = addResult.Value;
        var verificationResult = client.VerifyWallet(wallet.Id, request.UserInfo);
        if (verificationResult.IsFailed)
            return verificationResult;

        return Result.Ok(new EntityId(wallet.Id.Value));
    }
}

