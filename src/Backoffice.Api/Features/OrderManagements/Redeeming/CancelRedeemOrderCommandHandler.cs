using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Orders;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Redeeming;

internal sealed class CancelRedeemOrderCommandHandler : ICommandHandler<CancelRedeemOrderCommand, EntityId>
{
    private readonly DataContext _context;

    public CancelRedeemOrderCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<EntityId>> Handle(CancelRedeemOrderCommand command, CancellationToken cancellationToken)
    {
        var order = await _context.RedeemOrders.FirstAsync(x => x.Id == command.OrderId && x.Type == OrderType.Redeem,
            cancellationToken);

        var cancellationResult = order.Cancel(command.Request.Reason, command.UserInfo);
        if (cancellationResult.IsFailed)
            return cancellationResult;

        return Result.Ok(new EntityId(order.Id.Value));
    }
}
