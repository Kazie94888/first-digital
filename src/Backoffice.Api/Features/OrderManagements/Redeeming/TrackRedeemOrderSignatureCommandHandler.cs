using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Backoffice.Api.Base.Options;
using SmartCoinOS.Domain.Orders;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Redeeming;

internal sealed class TrackRedeemOrderSignatureCommandHandler : ICommandHandler<TrackRedeemOrderSignatureCommand, EntityId>
{
    readonly DataContext _context;
    readonly BlockchainSettings _settings;

    public TrackRedeemOrderSignatureCommandHandler(DataContext context, IOptionsSnapshot<BlockchainSettings> optionSettings)
    {
        _context = context;
        _settings = optionSettings.Value;
    }

    public async Task<Result<EntityId>> Handle(TrackRedeemOrderSignatureCommand command, CancellationToken cancellationToken)
    {
        var order = await _context.RedeemOrders.FirstAsync(x => x.Id == command.OrderId, cancellationToken);
        var request = command.Request;

        var safeSignatures = BuildSafeSignatures(request.Signatures);
        var signResult = order.Signed(request.SafeTxHash, safeSignatures, command.UserInfo);
        if (signResult.IsFailed)
            return signResult;

        return Result.Ok(new EntityId(order.Id.Value));
    }

    private List<SafeSignature> BuildSafeSignatures(List<SignatureDto> signatureDto)
    {
        var result = new List<SafeSignature>();
        foreach (var item in signatureDto)
        {
            var alias = _settings.Signers.GetValueOrDefault(item.Address);
            result.Add(new SafeSignature(item.Address, alias, item.SubmissionDate));
        }

        return result;
    }
}