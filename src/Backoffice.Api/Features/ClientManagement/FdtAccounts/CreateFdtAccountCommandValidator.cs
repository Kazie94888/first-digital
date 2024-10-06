using System.Text.RegularExpressions;
using FluentValidation;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.FdtAccounts;

public sealed class CreateFdtAccountCommandValidator : AbstractValidator<CreateFdtAccountCommand>
{
    public CreateFdtAccountCommandValidator()
    {
        RuleFor(x => x.Account.ClientName)
            .NotEmpty()
            .Length(10, 250);


        var matchOnlyNumbers = new Regex($"^[0-9]*$", RegexOptions.ECMAScript, TimeSpan.FromSeconds(1));

        RuleFor(x => x.Account.AccountNumber)
            .NotEmpty()
            .Length(12, 13)
            .Matches(matchOnlyNumbers)
            .WithMessage("Account number can contain only digits");
    }
}
