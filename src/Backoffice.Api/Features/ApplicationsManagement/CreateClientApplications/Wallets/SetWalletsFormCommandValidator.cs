using FluentValidation;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.Wallets;

public sealed class SetWalletsFormCommandValidator : AbstractValidator<SetWalletsFormCommand>
{
    public SetWalletsFormCommandValidator()
    {
        RuleForEach(x => x.Wallets)
            .NotEmpty()
            .ChildRules(c =>
            {
                c.RuleFor(x => x.Address).NotEmpty().Length(10, 100);
                c.RuleFor(x => x.Network).NotEmpty();
            });
    }
}
