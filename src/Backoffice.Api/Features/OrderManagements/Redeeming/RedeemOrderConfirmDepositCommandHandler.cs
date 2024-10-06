using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Redeeming;

internal sealed class RedeemOrderConfirmDepositCommandHandler : ICommandHandler<RedeemOrderConfirmDepositCommand, EntityId>
{
    readonly DataContext _context;

    public RedeemOrderConfirmDepositCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<EntityId>> Handle(RedeemOrderConfirmDepositCommand command, CancellationToken cancellationToken)
    {
        var order = await _context.RedeemOrders.FirstAsync(x => x.Id == command.OrderId, cancellationToken);
        var confirmResult = order.ConfirmDeposit(command.Request.DepositAmount, command.UserInfo);

        if (confirmResult.IsFailed)
            return confirmResult;

        return Result.Ok(new EntityId(order.Id.Value));
    }
}