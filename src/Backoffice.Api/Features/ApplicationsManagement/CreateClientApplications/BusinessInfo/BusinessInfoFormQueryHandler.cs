using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Application.Enums;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.BusinessInfo;

internal sealed class BusinessInfoFormQueryHandler : IQueryHandler<BusinessInfoFormQuery, ApplicationFormDto>
{
    private readonly ReadOnlyDataContext _context;

    public BusinessInfoFormQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<ApplicationFormDto>> Handle(BusinessInfoFormQuery request,
        CancellationToken cancellationToken)
    {
        var application = await _context.Applications.FirstAsync(x => x.Id == request.ApplicationId, cancellationToken);
        var businessInfoForm = application.BusinessInfo;

        var sourcesOfWealthSection = new VerifiableSectionDto<SourcesOfWealthRequest>();
        var pepSection = new VerifiableSectionDto<PoliticallyExposedPersonRequest>();
        var linesOfBusinessSection = new VerifiableSectionDto<LinesOfBusinessRequest>();
        var businessActivitiesSection = new VerifiableSectionDto<BusinessActivitiesRequest>();
        var projectedOrdersSection = new VerifiableSectionDto<ProjectedOrdersRequest>();

        if (businessInfoForm?.LinesOfBusiness is not null)
        {
            sourcesOfWealthSection.Set(businessInfoForm.SourcesOfWealth.IsVerified(), new SourcesOfWealthRequest
            {
                ContributionsOfMembersOrShareHolders = businessInfoForm.SourcesOfWealth.ContributionsOfMembersOrShareHolders,
                CapitalGainsOrDividends = businessInfoForm.SourcesOfWealth.CapitalGainsOrDividends,
                FinancialInstrumentOrInvestments = businessInfoForm.SourcesOfWealth.FinancialInstrumentOrInvestments,
                IncomeFromSaleOfAssets = businessInfoForm.SourcesOfWealth.IncomeFromSaleOfAssets,
                SettlementOfAccountsOrServicePayments = businessInfoForm.SourcesOfWealth.SettlementOfAccountsOrServicePayments,
                Loan = businessInfoForm.SourcesOfWealth.Loan,
                OtherSources = businessInfoForm.SourcesOfWealth.OtherSources,
                OtherSourcesDetails = businessInfoForm.SourcesOfWealth.OtherSourcesDetails,
                FurtherInformation = businessInfoForm.SourcesOfWealth.FurtherInformation,
                Documents = businessInfoForm.SourcesOfWealth.Documents.Select(x => (DocumentDto)x).ToList()
            });
            linesOfBusinessSection.Set(businessInfoForm.LinesOfBusiness.IsVerified(), new LinesOfBusinessRequest
            {
                BusinessEntityType = businessInfoForm.LinesOfBusiness.BusinessEntityType,
                AccountingOrFinancialServices = businessInfoForm.LinesOfBusiness.AccountingOrFinancialServices,
                AdministrativeAndSupportServiceActivities = businessInfoForm.LinesOfBusiness.AdministrativeAndSupportServiceActivities,
                ConsultingOrProfessionalServices = businessInfoForm.LinesOfBusiness.ConsultingOrProfessionalServices,
                ConsumerGoodsAndServices = businessInfoForm.LinesOfBusiness.ConsumerGoodsAndServices,
                ECommerce = businessInfoForm.LinesOfBusiness.ECommerce,
                EducationInstitutionOrServices = businessInfoForm.LinesOfBusiness.EducationInstitutionOrServices,
                Gambling = businessInfoForm.LinesOfBusiness.Gambling,
                GamingAndEsports = businessInfoForm.LinesOfBusiness.GamingAndEsports,
                Government = businessInfoForm.LinesOfBusiness.Government,
                HotelsRestaurantsOrLeisure = businessInfoForm.LinesOfBusiness.HotelsRestaurantsOrLeisure,
                HumanResourcesOrEmploymentServices = businessInfoForm.LinesOfBusiness.HumanResourcesOrEmploymentServices,
                ManagementOfOwnAssets = businessInfoForm.LinesOfBusiness.ManagementOfOwnAssets,
                MaterialsManufacturingAndConstruction = businessInfoForm.LinesOfBusiness.MaterialsManufacturingAndConstruction,
                NonProfitOrganization = businessInfoForm.LinesOfBusiness.NonProfitOrganization,
                ProfessionalScientificAndTechnicalActivities = businessInfoForm.LinesOfBusiness.ProfessionalScientificAndTechnicalActivities,
                RealEstateActivities = businessInfoForm.LinesOfBusiness.RealEstateActivities,
                TravelTransportationOrAutomotive = businessInfoForm.LinesOfBusiness.TravelTransportationOrAutomotive,
                SoftwareItOrHardware = businessInfoForm.LinesOfBusiness.SoftwareItOrHardware,
                MediaAndAdvertising = businessInfoForm.LinesOfBusiness.MediaAndAdvertising,
                OtherSources = businessInfoForm.LinesOfBusiness.OtherSources,
                OtherSourcesDetails = businessInfoForm.LinesOfBusiness.OtherSourcesDetails
            });
            pepSection.Set(businessInfoForm.PoliticallyExposedPerson.IsVerified(), new PoliticallyExposedPersonRequest
            {
                IsPep = businessInfoForm.PoliticallyExposedPerson.IsPep,
                PepDetails = businessInfoForm.PoliticallyExposedPerson.PepDetails,
                IsFamilyMemberPep = businessInfoForm.PoliticallyExposedPerson.IsFamilyMemberPep,
                FamilyMemberPepDetails = businessInfoForm.PoliticallyExposedPerson.FamilyMemberPepDetails,
                IsCloseAssociatePep = businessInfoForm.PoliticallyExposedPerson.IsCloseAssociatePep,
                CloseAssociatePepDetails = businessInfoForm.PoliticallyExposedPerson.CloseAssociatePepDetails
            });
            businessActivitiesSection.Set(businessInfoForm.BusinessActivities.IsVerified(), new BusinessActivitiesRequest
            {
                Description = businessInfoForm.BusinessActivities.Description,
                IsFinancialStatementsRequired = businessInfoForm.BusinessActivities.IsFinancialStatementsRequired,
                FinancialStatementsDetails = businessInfoForm.BusinessActivities.FinancialStatementsDetails,
                HasHongKongBusinessActivities = businessInfoForm.BusinessActivities.HasHongKongBusinessActivities,
                HongKongBusinessActivitiesDetails = businessInfoForm.BusinessActivities.HongKongBusinessActivitiesDetails,
                IsTaxResidentInCorpOriginCountry = businessInfoForm.BusinessActivities.IsTaxResidentInCorpOriginCountry,
                TaxResidentInCorpOriginCountryDetails = businessInfoForm.BusinessActivities.TaxResidentInCorpOriginCountryDetails,
                HasTaxResidentInOtherCountries = businessInfoForm.BusinessActivities.HasTaxResidentInOtherCountries,
                OtherTaxResidencyCountries = businessInfoForm.BusinessActivities.OtherTaxResidencyCountries,
                OtherCountryTins = businessInfoForm.BusinessActivities.OtherCountryTins,
                IsSpecialPermissionRequired = businessInfoForm.BusinessActivities.IsSpecialPermissionRequired,
                SpecialPermissionDetails = businessInfoForm.BusinessActivities.SpecialPermissionDetails,
                ApproximateNumberOfEmployees = businessInfoForm.BusinessActivities.ApproximateNumberOfEmployees,
                AnnualNetTurnover = businessInfoForm.BusinessActivities.AnnualNetTurnover
            });
            projectedOrdersSection.Set(businessInfoForm.ProjectedOrders.IsVerified(), new ProjectedOrdersRequest
            {
                ExpectedMonthlyStableCoinsMintedAmount = businessInfoForm.ProjectedOrders.ExpectedMonthlyStableCoinsMintedAmount,
                ExpectedMonthlyCumulativeMints = businessInfoForm.ProjectedOrders.ExpectedMonthlyCumulativeMints,
                MaxAnticipatedAmountForSingleMint = businessInfoForm.ProjectedOrders.MaxAnticipatedAmountForSingleMint,
                ExpectedIrregularMintsFirstYear = businessInfoForm.ProjectedOrders.ExpectedIrregularMintsFirstYear
            });
        }

        var applicationForm = new ApplicationFormDto(ApplicationFormType.BusinessInfo);
        applicationForm.AddSection(ApplicationFormNames.BusinessInfoRecords.SourceOfWealth, sourcesOfWealthSection);
        applicationForm.AddSection(ApplicationFormNames.BusinessInfoRecords.PoliticallyExposedPerson, pepSection);
        applicationForm.AddSection(ApplicationFormNames.BusinessInfoRecords.LinesOfBusiness, linesOfBusinessSection);
        applicationForm.AddSection(ApplicationFormNames.BusinessInfoRecords.BusinessActivities, businessActivitiesSection);
        applicationForm.AddSection(ApplicationFormNames.BusinessInfoRecords.ProjectedOrders, projectedOrdersSection);

        return Result.Ok(applicationForm);
    }
}