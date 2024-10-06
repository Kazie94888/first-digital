using FluentValidation;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Minting;

public sealed class RsnReferenceAddCommandValidator : AbstractValidator<RsnReferenceAddCommand>
{
    public RsnReferenceAddCommandValidator()
    {
        RuleFor(x => x.Request.RsnReference).NotEmpty();
    }
}
