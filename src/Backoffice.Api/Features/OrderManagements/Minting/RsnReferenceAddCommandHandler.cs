using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Orders;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Minting;

internal sealed class RsnReferenceAddCommandHandler : ICommandHandler<RsnReferenceAddCommand, EntityId>
{
    private readonly DataContext _context;

    public RsnReferenceAddCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<EntityId>> Handle(RsnReferenceAddCommand command, CancellationToken cancellationToken)
    {
        var order = await _context.MintOrders.FirstAsync(x => x.Id == command.OrderId, cancellationToken);

        var setRsnResult = order.AddRsnReference(new RsnReference(command.Request.RsnReference), command.UserInfo);
        if (setRsnResult.IsFailed)
            return setRsnResult;

        return Result.Ok(new EntityId(order.Id.Value));
    }
}