using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Orders;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Redeeming;

internal sealed class RedeemOrderAddRsnReferenceCommandHandler : ICommandHandler<RedeemOrderAddRsnReferenceCommand, EntityId>
{
    private readonly DataContext _context;

    public RedeemOrderAddRsnReferenceCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<EntityId>> Handle(RedeemOrderAddRsnReferenceCommand command, CancellationToken cancellationToken)
    {
        var order = await _context.RedeemOrders.FirstAsync(x => x.Id == command.OrderId, cancellationToken);

        var setRsnResult = order.AddRsnReference(new RsnReference(command.Request.RsnReference), command.UserInfo);
        if (setRsnResult.IsFailed)
            return setRsnResult;

        return Result.Ok(new EntityId(order.Id.Value));
    }
}