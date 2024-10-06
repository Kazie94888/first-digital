using FluentValidation;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Minting;

public sealed class RsnAmountAddCommandValidator : AbstractValidator<RsnAmountAddCommand>
{
    public RsnAmountAddCommandValidator()
    {
        RuleFor(x => x.Request.Amount).NotEmpty();
        RuleFor(x => x.Request.Amount.Amount).GreaterThan(decimal.Zero);
    }
}