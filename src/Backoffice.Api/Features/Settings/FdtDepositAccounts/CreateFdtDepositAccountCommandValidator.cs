using System.Text.RegularExpressions;
using FluentValidation;

namespace SmartCoinOS.Backoffice.Api.Features.Settings.FdtDepositAccounts;

public sealed class CreateFdtDepositAccountCommandValidator : AbstractValidator<CreateFdtDepositAccountCommand>
{
    public CreateFdtDepositAccountCommandValidator()
    {
        RuleFor(x => x.FdtDepositAccount.AccountName)
            .NotEmpty()
            .Length(10, 250);

        var matchOnlyNumbers = new Regex($"^[0-9]*$", RegexOptions.ECMAScript, TimeSpan.FromSeconds(1));
        RuleFor(x => x.FdtDepositAccount.AccountNumber)
            .NotEmpty()
            .Length(12, 13)
            .Matches(matchOnlyNumbers)
            .WithMessage("Account number can contain only digits");
    }
}
