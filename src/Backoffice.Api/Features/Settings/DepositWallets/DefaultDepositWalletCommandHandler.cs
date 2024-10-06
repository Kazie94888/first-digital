using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.Settings.DepositWallets;

internal sealed class DefaultDepositWalletCommandHandler : ICommandHandler<DefaultDepositWalletCommand, EntityId>
{
    private readonly DataContext _context;

    public DefaultDepositWalletCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<EntityId>> Handle(DefaultDepositWalletCommand request, CancellationToken cancellationToken)
    {
        var depositWallet = await _context.DepositWallets.FirstAsync(x => x.Id == request.Id, cancellationToken);

        var defaultResult = depositWallet.MakeDefault(request.UserInfo);
        if (defaultResult.IsFailed)
            return defaultResult;

        return Result.Ok(new EntityId(depositWallet.Id.Value));
    }
}