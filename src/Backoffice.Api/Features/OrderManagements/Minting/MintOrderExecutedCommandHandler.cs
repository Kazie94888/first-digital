using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Backoffice.Api.Base.Options;
using SmartCoinOS.Domain.Orders;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Minting;

internal sealed class MintOrderExecutedCommandHandler : ICommandHandler<MintOrderExecutedCommand, EntityId>
{
    readonly DataContext _context;
    readonly BlockchainSettings _settings;

    public MintOrderExecutedCommandHandler(DataContext context, IOptionsSnapshot<BlockchainSettings> settings)
    {
        _context = context;
        _settings = settings.Value;
    }

    public async Task<Result<EntityId>> Handle(MintOrderExecutedCommand command, CancellationToken cancellationToken)
    {
        var order = await _context.MintOrders.FirstAsync(x => x.Id == command.OrderId, cancellationToken);
        var request = command.Request;

        var alias = _settings.Signers.GetValueOrDefault(request.Signature.Address);
        var safeSignature = new SafeSignature(request.Signature.Address, alias, request.Signature.SubmissionDate);

        var executionResult = order.Executed(safeSignature, command.UserInfo);
        if (executionResult.IsFailed)
            return executionResult;

        return Result.Ok(new EntityId(order.Id.Value));
    }
}
