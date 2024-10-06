using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Domain.Deposit.Events;
using SmartCoinOS.Persistence;

namespace SmartCoinOS.Commands.EventHandlers;

internal class DepositWalletMadeDefaultEventHandler : INotificationHandler<DepositWalletMadeDefaultEvent>
{
    private readonly DataContext _context;

    public DepositWalletMadeDefaultEventHandler(DataContext context)
    {
        _context = context;
    }

    public async Task Handle(DepositWalletMadeDefaultEvent notification, CancellationToken cancellationToken)
    {
        var previousDefault = await _context.DepositWallets
            .FirstOrDefaultAsync(x => x.Id != notification.Id && x.Default, cancellationToken);

        if (previousDefault is null)
            return;

        previousDefault.RemoveDefaultFlag();
    }
}