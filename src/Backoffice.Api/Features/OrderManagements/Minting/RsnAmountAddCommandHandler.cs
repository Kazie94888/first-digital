using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Minting;

internal sealed class RsnAmountAddCommandHandler : ICommandHandler<RsnAmountAddCommand, EntityId>
{

    private readonly DataContext _context;

    public RsnAmountAddCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<EntityId>> Handle(RsnAmountAddCommand command, CancellationToken cancellationToken)
    {
        var order = await _context.MintOrders.FirstAsync(x => x.Id == command.OrderId, cancellationToken);

        var setRsnAmountResult = order.AddRsnAmount(command.Request.Amount, command.UserInfo);
        if (setRsnAmountResult.IsFailed)
            return setRsnAmountResult;

        return Result.Ok(new EntityId(order.Id.Value));
    }
}