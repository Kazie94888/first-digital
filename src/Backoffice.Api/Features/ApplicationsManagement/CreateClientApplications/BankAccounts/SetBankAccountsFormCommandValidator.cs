using FluentValidation;
using SmartCoinOS.Domain.Services;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.BankAccounts;

public sealed class SetBankAccountsFormCommandValidator : AbstractValidator<SetBankAccountsFormCommand>
{
    public SetBankAccountsFormCommandValidator()
    {
        RuleFor(x => x.BankAccounts).NotEmpty();

        var documentTypeService = DocumentTypeService.Instance;
        RuleForEach(b => b.BankAccounts)
            .ChildRules(c =>
            {
                c.RuleFor(x => x.Beneficiary).MaximumLength(100).NotEmpty();
                c.RuleFor(x => x.BankName).MaximumLength(100).NotEmpty();
                c.RuleFor(x => x.Iban).MaximumLength(34).NotEmpty();
                c.RuleFor(x => x.Swift).MaximumLength(11).NotEmpty();

                c.RuleFor(x => x.Street).MaximumLength(100).NotEmpty();
                c.RuleFor(x => x.City).MaximumLength(100).NotEmpty();

                c.RuleFor(x => x.PostalCode).NotEmpty().MaximumLength(100);
                c.RuleFor(x => x.State).MaximumLength(100);

                c.RuleFor(x => x.Country).NotEmpty();

                c.RuleFor(x => x.SmartTrustOwnBankId)
                .NotEmpty()
                .When(x => x.SmartTrustThirdPartyBankId is null)
                .WithMessage("Bank should have either OwnBankId or ThirdPartyId");

                c.RuleFor(x => x.SmartTrustThirdPartyBankId)
                .NotEmpty()
                .When(x => x.SmartTrustOwnBankId is null)
                .WithMessage("Bank should have either OwnBankId or ThirdPartyId");

                c.RuleFor(x => x.Documents).NotEmpty();
                c.RuleForEach(x => x.Documents).Must(x => documentTypeService.IsBankDocument(x.DocumentType));
            });
    }
}