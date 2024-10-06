using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Redeeming;

internal sealed class RedeemOrderCompleteCommandHandler : ICommandHandler<RedeemOrderCompleteCommand, EntityId>
{
    readonly DataContext _context;

    public RedeemOrderCompleteCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<EntityId>> Handle(RedeemOrderCompleteCommand command, CancellationToken cancellationToken)
    {
        var order = await _context.RedeemOrders.FirstAsync(x => x.Id == command.OrderId, cancellationToken);

        var completeResult = order.Complete(command.UserInfo);
        if (completeResult.IsFailed)
            return completeResult;

        return Result.Ok(new EntityId(order.Id.Value));
    }
}