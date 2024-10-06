using FluentValidation;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Redeeming;

public sealed class RedeemOrderSetTxHashCommandValidator : AbstractValidator<RedeemOrderSetTxHashCommand>
{
    public RedeemOrderSetTxHashCommandValidator()
    {
        RuleFor(x => x.Request).NotEmpty();
        RuleFor(x => x.Request.TxHash).NotEmpty();
    }
}
