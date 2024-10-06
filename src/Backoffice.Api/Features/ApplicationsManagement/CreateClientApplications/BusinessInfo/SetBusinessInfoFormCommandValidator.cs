using FluentValidation;
using SmartCoinOS.Domain.Services;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.BusinessInfo;

public sealed class SetBusinessInfoFormCommandValidator : AbstractValidator<SetBusinessInfoFormCommand>
{
    public SetBusinessInfoFormCommandValidator()
    {
        RuleFor(x => x.BusinessInfo.SourcesOfWealth).NotNull();
        RuleFor(x => x.BusinessInfo.PoliticallyExposedPerson).NotNull();
        RuleFor(x => x.BusinessInfo.LinesOfBusiness).NotNull();
        RuleFor(x => x.BusinessInfo.BusinessActivities).NotNull();
        RuleFor(x => x.BusinessInfo.ProjectedOrders).NotNull();

        var documentTypeService = DocumentTypeService.Instance;
        RuleFor(x => x.BusinessInfo.SourcesOfWealth.OtherSourcesDetails)
            .NotEmpty()
            .When(x => x.BusinessInfo.SourcesOfWealth.OtherSources);
        RuleForEach(x => x.BusinessInfo.SourcesOfWealth.Documents)
            .Must(x => documentTypeService.IsSourceOfWealth(x.DocumentType));

        RuleFor(x => x.BusinessInfo.SourcesOfWealth.FurtherInformation).NotEmpty();

        RuleFor(x => x.BusinessInfo.PoliticallyExposedPerson.PepDetails)
            .NotEmpty()
            .When(x => x.BusinessInfo.PoliticallyExposedPerson.IsPep);
        RuleFor(x => x.BusinessInfo.PoliticallyExposedPerson.FamilyMemberPepDetails)
            .NotEmpty()
            .When(x => x.BusinessInfo.PoliticallyExposedPerson.IsFamilyMemberPep);
        RuleFor(x => x.BusinessInfo.PoliticallyExposedPerson.CloseAssociatePepDetails)
            .NotEmpty()
            .When(x => x.BusinessInfo.PoliticallyExposedPerson.IsCloseAssociatePep);

        RuleFor(x => x.BusinessInfo.LinesOfBusiness.OtherSourcesDetails)
            .NotEmpty()
            .When(x => x.BusinessInfo.LinesOfBusiness.OtherSources);

        RuleFor(x => x.BusinessInfo.BusinessActivities.Description).NotEmpty();
        RuleFor(x => x.BusinessInfo.BusinessActivities.FinancialStatementsDetails)
            .NotEmpty()
            .When(x => x.BusinessInfo.BusinessActivities.IsFinancialStatementsRequired);
        RuleFor(x => x.BusinessInfo.BusinessActivities.HongKongBusinessActivitiesDetails)
            .NotEmpty()
            .When(x => x.BusinessInfo.BusinessActivities.HasHongKongBusinessActivities);
        RuleFor(x => x.BusinessInfo.BusinessActivities.TaxResidentInCorpOriginCountryDetails)
            .NotEmpty()
            .When(x => x.BusinessInfo.BusinessActivities.IsTaxResidentInCorpOriginCountry);
        RuleFor(x => x.BusinessInfo.BusinessActivities.OtherTaxResidencyCountries)
            .NotEmpty()
            .When(x => x.BusinessInfo.BusinessActivities.HasTaxResidentInOtherCountries);
        RuleFor(x => x.BusinessInfo.BusinessActivities.OtherCountryTins)
            .NotEmpty()
            .When(x => x.BusinessInfo.BusinessActivities.HasTaxResidentInOtherCountries);
        RuleFor(x => x.BusinessInfo.BusinessActivities.SpecialPermissionDetails)
            .NotEmpty()
            .When(x => x.BusinessInfo.BusinessActivities.IsSpecialPermissionRequired);
    }
}