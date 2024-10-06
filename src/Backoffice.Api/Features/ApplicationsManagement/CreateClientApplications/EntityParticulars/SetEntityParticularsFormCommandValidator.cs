using System.Text.RegularExpressions;
using FluentValidation;
using SmartCoinOS.Domain.Application;
using SmartCoinOS.Domain.Services;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.EntityParticulars;

public sealed class SetEntityParticularsFormCommandValidator : AbstractValidator<SetEntityParticularsFormCommand>
{
    public SetEntityParticularsFormCommandValidator()
    {
        RuleFor(x => x.EntityParticularsForm.CompanyParticulars).NotEmpty();
        RuleFor(x => x.EntityParticularsForm.RegistrationAddress).NotEmpty();
        RuleFor(x => x.EntityParticularsForm.CompanyContact).NotEmpty();

        RuleFor(x => x.EntityParticularsForm.CompanyParticulars.LegalStructure)
            .NotEmpty();

        var documentTypeService = DocumentTypeService.Instance;
        RuleFor(x => x.EntityParticularsForm.CompanyParticulars.Documents).NotEmpty();
        RuleForEach(x => x.EntityParticularsForm.CompanyParticulars.Documents)
            .Must(x => documentTypeService.IsCompanyParticular(x.DocumentType));

        RuleFor(x => x.EntityParticularsForm.CompanyParticulars.RegistrationNumber).NotEmpty();
        RuleFor(x => x.EntityParticularsForm.CompanyParticulars.DateOfInc).NotEmpty();
        RuleFor(x => x.EntityParticularsForm.CompanyParticulars.CountryOfInc)
            .MaximumLength(100)
            .NotEmpty();

        RuleFor(x => x.EntityParticularsForm.CompanyParticulars.StructureDetails)
                .NotEmpty()
                .When(x => x.EntityParticularsForm.CompanyParticulars.LegalStructure == CompanyLegalStructure.Other);

        RuleFor(x => x.EntityParticularsForm.RegistrationAddress.Street).NotEmpty().Length(3, 250);
        RuleFor(x => x.EntityParticularsForm.RegistrationAddress.City).NotEmpty().Length(3, 250);
        RuleFor(x => x.EntityParticularsForm.RegistrationAddress.Country).NotEmpty().MaximumLength(100);
        RuleFor(x => x.EntityParticularsForm.RegistrationAddress.PostalCode).NotEmpty().MaximumLength(11);
        RuleFor(x => x.EntityParticularsForm.RegistrationAddress.State).MaximumLength(100);

        RuleFor(x => x.EntityParticularsForm.CompanyContact.Email).NotEmpty().MaximumLength(250);

        var matchOnlyPhoneNumbers = new Regex("^[\\d,.;:_)(-]{1,}$", RegexOptions.ECMAScript, TimeSpan.FromSeconds(1));

        RuleFor(x => x.EntityParticularsForm.CompanyContact.Phone)
            .NotEmpty()
            .Matches(matchOnlyPhoneNumbers)
            .WithMessage("Only digits, '-' and '()' are allowed")
            .MaximumLength(50);

        RuleFor(x => x.EntityParticularsForm.CompanyContact.Email)
            .NotEmpty()
            .EmailAddress();
    }
}
