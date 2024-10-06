using FluentValidation;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.BankAccounts;

public sealed class CreateBankAccountCommandValidator : AbstractValidator<CreateBankAccountCommand>
{
    public CreateBankAccountCommandValidator()
    {
        RuleFor(x => x.BankAccount.BankAccountDetails.Beneficiary)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(x => x.BankAccount.BankAccountDetails.Iban)
            .NotEmpty()
            .MaximumLength(34);

        RuleFor(x => x.BankAccount.BankAccountDetails.BankName)
            .NotEmpty()
            .MaximumLength(250);

        RuleFor(x => x.BankAccount.BankAccountDetails.SwiftCode)
            .NotEmpty()
            .MaximumLength(11);

        RuleFor(x => x.BankAccount.BankAccountDetails.Alias)
            .MaximumLength(100);

        RuleFor(x => x.BankAccount.BankAddress.Street)
            .NotEmpty()
            .MaximumLength(250);

        RuleFor(x => x.BankAccount.BankAddress.City)
            .NotEmpty()
            .MaximumLength(250);

        RuleFor(x => x.BankAccount.BankAddress.PostalCode)
            .NotEmpty()
            .MaximumLength(15);

        RuleFor(x => x.BankAccount.BankAddress.Country)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.BankAccount.BankAddress.State)
            .MaximumLength(250);
    }
}
