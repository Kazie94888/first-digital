using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Wallets;

internal class WalletArchivingCommandHandler : ICommandHandler<WalletArchivingCommand, EntityId>
{
    private readonly DataContext _context;

    public WalletArchivingCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<EntityId>> Handle(WalletArchivingCommand archivingCommand, CancellationToken cancellationToken)
    {
        var client = await _context.Clients.FirstAsync(x => x.Id == archivingCommand.ClientId, cancellationToken);

        var hasActiveOrders = false;
        // remaining: operation should only continue if there's no order in progress using this bank account

        var archiveResult = client.ArchiveWallet(archivingCommand.WalletId, hasActiveOrders, archivingCommand.UserInfo);
        if (archiveResult.IsFailed)
            return archiveResult;

        return Result.Ok(new EntityId(archivingCommand.WalletId.Value));
    }
}