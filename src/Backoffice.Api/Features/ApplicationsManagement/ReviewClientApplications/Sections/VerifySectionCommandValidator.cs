using FluentValidation;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.ReviewClientApplications.Sections;

public sealed class VerifySectionCommandValidator : AbstractValidator<VerifySectionCommand>
{
    public VerifySectionCommandValidator()
    {
        RuleFor(x => x.ApplicationId).NotEmpty();
        RuleFor(x => x.Request.Form).IsInEnum();

        RuleFor(x => x.Request.Record).NotEmpty().When(x => string.IsNullOrEmpty(x.Request.Id));
        RuleFor(x => x.Request.Id).NotEmpty().When(x => string.IsNullOrEmpty(x.Request.Record));

        RuleFor(x => x.Request.Record)
            .Must((command, record) =>
            {
                return string.IsNullOrEmpty(command.Request.Id) || string.IsNullOrEmpty(command.Request.Record);
            }).WithMessage("You should specify either a record, or an id. Not both.");
    }
}