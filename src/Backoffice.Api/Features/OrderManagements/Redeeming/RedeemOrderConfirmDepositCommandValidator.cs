using FluentValidation;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Redeeming;

public sealed class RedeemOrderConfirmDepositCommandValidator : AbstractValidator<RedeemOrderConfirmDepositCommand>
{
    public RedeemOrderConfirmDepositCommandValidator()
    {
        RuleFor(x => x.Request).NotEmpty();
        RuleFor(x => x.Request.DepositAmount).NotEmpty();
    }
}
