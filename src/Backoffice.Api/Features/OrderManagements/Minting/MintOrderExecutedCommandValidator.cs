using FluentValidation;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Minting;

public sealed class MintOrderExecutedCommandValidator : AbstractValidator<MintOrderExecutedCommand>
{
    public MintOrderExecutedCommandValidator()
    {
        RuleFor(x => x.Request.Signature).NotEmpty();
        RuleFor(x => x.Request.Signature).ChildRules(sign =>
        {
            sign.RuleFor(x => x.Address).NotEmpty();
            sign.RuleFor(x => x.SubmissionDate).NotEmpty();
        });
    }
}