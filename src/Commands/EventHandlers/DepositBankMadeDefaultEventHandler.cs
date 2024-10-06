using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Domain.Deposit.Events;
using SmartCoinOS.Persistence;

namespace SmartCoinOS.Commands.EventHandlers;
internal class DepositBankMadeDefaultEventHandler : INotificationHandler<DepositBankMadeDefaultEvent>
{
    private readonly DataContext _context;

    public DepositBankMadeDefaultEventHandler(DataContext context)
    {
        _context = context;
    }

    public async Task Handle(DepositBankMadeDefaultEvent notification, CancellationToken cancellationToken)
    {
        var previousDefault = await _context.DepositBanks.FirstOrDefaultAsync(x => x.Id != notification.Id && x.Default, cancellationToken);

        if (previousDefault is null)
            return;

        previousDefault.RemoveDefaultFlag();
    }
}