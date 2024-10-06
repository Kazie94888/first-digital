using System.Text.RegularExpressions;
using FluentValidation;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.FdtAccounts;

public sealed class SetFdtAccountsFormCommandValidator : AbstractValidator<SetFdtAccountsFormCommand>
{
    public SetFdtAccountsFormCommandValidator()
    {
        RuleForEach(x => x.FdtAccounts)
                .NotEmpty()
                .ChildRules(c =>
                {
                    c.RuleFor(x => x.ClientName)
                        .NotEmpty()
                        .Length(10, 250);


                    var matchOnlyNumbers = new Regex($"^[0-9]*$", RegexOptions.ECMAScript, TimeSpan.FromSeconds(1));

                    c.RuleFor(x => x.AccountNumber)
                        .NotEmpty()
                        .Length(12, 13)
                        .Matches(matchOnlyNumbers)
                        .WithMessage("Account number can contain only digits");
                });
    }
}