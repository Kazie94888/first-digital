using FluentValidation;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Redeeming;

public sealed class CancelRedeemOrderCommandValidator : AbstractValidator<CancelRedeemOrderCommand>
{
    public CancelRedeemOrderCommandValidator()
    {
        RuleFor(x => x.Request).NotNull()
            .ChildRules(c =>
            {
                c.RuleFor(x => x.Reason).NotEmpty().MaximumLength(100);
            });
    }
}