using FluentValidation;

namespace SmartCoinOS.Backoffice.Api.Features.Settings.DepositBanks;

public sealed class CreateDepositBankCommandValidator : AbstractValidator<CreateDepositBankCommand>
{
    public CreateDepositBankCommandValidator()
    {
        RuleFor(x => x.Request.DepositBankAccount.Beneficiary)
             .NotEmpty()
             .Length(3, 100);

        RuleFor(x => x.Request.DepositBankAccount.Iban)
            .NotEmpty()
            .MaximumLength(34);

        RuleFor(x => x.Request.DepositBankAccount.BankName)
            .NotEmpty()
            .MaximumLength(250);

        RuleFor(x => x.Request.DepositBankAccount.SwiftCode)
            .NotEmpty()
            .MaximumLength(11);

        RuleFor(x => x.Request.DepositBankAddress.Street)
            .NotEmpty()
            .MaximumLength(250);

        RuleFor(x => x.Request.DepositBankAddress.City)
            .NotEmpty()
            .MaximumLength(250);

        RuleFor(x => x.Request.DepositBankAddress.PostalCode)
            .NotEmpty()
            .MaximumLength(15);

        RuleFor(x => x.Request.DepositBankAddress.Country)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Request.DepositBankAddress.State)
            .MaximumLength(250);
    }
}
