using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Redeeming;

internal sealed class RedeemOrderSetTxHashCommandHandler : ICommandHandler<RedeemOrderSetTxHashCommand, EntityId>
{
    private readonly DataContext _context;

    public RedeemOrderSetTxHashCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<EntityId>> Handle(RedeemOrderSetTxHashCommand command, CancellationToken cancellationToken)
    {
        var order = await _context.RedeemOrders.FirstAsync(x => x.Id == command.OrderId, cancellationToken);
        var setResult = order.SetTransactionHash(command.Request.TxHash, command.UserInfo);

        if (setResult.IsFailed)
            return setResult;

        return Result.Ok(new EntityId(order.Id.Value));
    }
}