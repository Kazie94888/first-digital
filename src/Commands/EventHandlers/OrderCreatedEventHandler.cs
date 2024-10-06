using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Domain.Orders.Events;
using SmartCoinOS.Persistence;

namespace SmartCoinOS.Commands.EventHandlers;
internal class OrderCreatedEventHandler : INotificationHandler<OrderCreatedEvent>
{
    private readonly DataContext _context;

    public OrderCreatedEventHandler(DataContext context)
    {
        _context = context;
    }

    public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
    {
        var ordersInThisWallet = await _context.Orders.Where(x => x.WalletId == notification.ClientWalletId).CountAsync(cancellationToken);

        var client = await _context.Clients.FirstAsync(x => x.Id == notification.ClientId, cancellationToken);
        client.SetWalletOrderCount(notification.ClientWalletId, ordersInThisWallet);

        _context.Update(client);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
