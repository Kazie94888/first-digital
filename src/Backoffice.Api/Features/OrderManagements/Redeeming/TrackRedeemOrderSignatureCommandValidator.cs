using FluentValidation;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Redeeming;

public sealed class TrackRedeemOrderSignatureCommandValidator : AbstractValidator<TrackRedeemOrderSignatureCommand>
{
    public TrackRedeemOrderSignatureCommandValidator()
    {
        RuleFor(x => x.Request.SafeTxHash).NotEmpty();
        RuleFor(x => x.Request.Signatures).NotEmpty();

        RuleForEach(x => x.Request.Signatures).ChildRules(sign =>
        {
            sign.RuleFor(s => s.Address).NotEmpty();
            sign.RuleFor(s => s.SubmissionDate).NotEmpty();
        });
    }
}