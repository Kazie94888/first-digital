using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Redeeming;

internal sealed class RedeemOrderPaymentCompletedCommandHandler : ICommandHandler<RedeemOrderPaymentCompletedCommand, EntityId>
{
    private readonly DataContext _context;

    public RedeemOrderPaymentCompletedCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<EntityId>> Handle(RedeemOrderPaymentCompletedCommand command, CancellationToken cancellationToken)
    {
        var order = await (from ord in _context.RedeemOrders
                           where ord.Type == Domain.Orders.OrderType.Redeem
                                   && ord.PaymentInstruction != null
                                   && ord.PaymentInstruction.Id == command.InstructionId
                                   && ord.PaymentInstruction.ReferenceNumber == command.ReferenceNumber
                           select ord).FirstOrDefaultAsync(cancellationToken);

        if (order is null)
            return Result.Fail($"Order '{command.ReferenceNumber}' couldn't be found.");

        var completeResult = order.PaymentInstructionCompleted(command.UserInfo);

        if (completeResult.IsFailed)
            return completeResult;

        return Result.Ok(new EntityId(order.Id.Value));
    }
}
