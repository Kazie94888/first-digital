using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Application;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.BusinessInfo;

public class SetBusinessInfoFormHandler : ICommandHandler<SetBusinessInfoFormCommand, ApplicationFormMetadataDto>
{
    private readonly DataContext _context;

    public SetBusinessInfoFormHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<ApplicationFormMetadataDto>> Handle(SetBusinessInfoFormCommand formCommand, CancellationToken cancellationToken)
    {
        var application = await _context.Applications.FirstAsync(x => x.Id == formCommand.ApplicationId, cancellationToken);

        var sourcesOfWealth = formCommand.BusinessInfo.SourcesOfWealth;
        var pep = formCommand.BusinessInfo.PoliticallyExposedPerson;
        var linesOfBusiness = formCommand.BusinessInfo.LinesOfBusiness;
        var businessActivities = formCommand.BusinessInfo.BusinessActivities;
        var projectedOrders = formCommand.BusinessInfo.ProjectedOrders;

        var documents = sourcesOfWealth.Documents.Select(x => (ApplicationDocument)x).ToList();

        var businessInfoBuilder = new BusinessInfoFormBuilder();

        businessInfoBuilder.AddSourcesOfWealth(sourcesOfWealth.ContributionsOfMembersOrShareHolders,
            sourcesOfWealth.CapitalGainsOrDividends, sourcesOfWealth.FinancialInstrumentOrInvestments,
            sourcesOfWealth.IncomeFromSaleOfAssets, sourcesOfWealth.SettlementOfAccountsOrServicePayments,
            sourcesOfWealth.Loan, sourcesOfWealth.OtherSources, sourcesOfWealth.OtherSourcesDetails,
            sourcesOfWealth.FurtherInformation, documents);

        businessInfoBuilder.AddPoliticallyExposedPerson(pep.IsPep, pep.PepDetails, pep.IsFamilyMemberPep,
            pep.FamilyMemberPepDetails, pep.IsCloseAssociatePep, pep.CloseAssociatePepDetails);

        businessInfoBuilder.AddLinesOfBusiness(linesOfBusiness.BusinessEntityType,
            linesOfBusiness.AccountingOrFinancialServices, linesOfBusiness.AdministrativeAndSupportServiceActivities,
            linesOfBusiness.ConsultingOrProfessionalServices, linesOfBusiness.ConsumerGoodsAndServices,
            linesOfBusiness.ECommerce, linesOfBusiness.EducationInstitutionOrServices, linesOfBusiness.Gambling,
            linesOfBusiness.GamingAndEsports, linesOfBusiness.Government, linesOfBusiness.HotelsRestaurantsOrLeisure,
            linesOfBusiness.HumanResourcesOrEmploymentServices, linesOfBusiness.ManagementOfOwnAssets,
            linesOfBusiness.MaterialsManufacturingAndConstruction, linesOfBusiness.NonProfitOrganization,
            linesOfBusiness.ProfessionalScientificAndTechnicalActivities, linesOfBusiness.RealEstateActivities,
            linesOfBusiness.TravelTransportationOrAutomotive, linesOfBusiness.SoftwareItOrHardware,
            linesOfBusiness.MediaAndAdvertising, sourcesOfWealth.OtherSources, sourcesOfWealth.OtherSourcesDetails);

        businessInfoBuilder.AddBusinessActivities(businessActivities.Description,
            businessActivities.IsFinancialStatementsRequired, businessActivities.FinancialStatementsDetails,
            businessActivities.HasHongKongBusinessActivities, businessActivities.HongKongBusinessActivitiesDetails,
            businessActivities.IsTaxResidentInCorpOriginCountry,
            businessActivities.TaxResidentInCorpOriginCountryDetails, businessActivities.HasTaxResidentInOtherCountries,
            businessActivities.OtherTaxResidencyCountries, businessActivities.OtherCountryTins,
            businessActivities.IsSpecialPermissionRequired, businessActivities.SpecialPermissionDetails,
            businessActivities.ApproximateNumberOfEmployees, businessActivities.AnnualNetTurnover);

        businessInfoBuilder.AddProjectedOrders(projectedOrders.ExpectedMonthlyStableCoinsMintedAmount,
            projectedOrders.ExpectedMonthlyCumulativeMints, projectedOrders.MaxAnticipatedAmountForSingleMint,
            projectedOrders.ExpectedIrregularMintsFirstYear);

        var businessInfoForm = businessInfoBuilder.Build();


        var result = application.SetBusinessInfo(businessInfoForm);

        if (result.IsFailed)
            return result;

        var response = new ApplicationFormMetadataDto
        {
            PrettyId = application.ApplicationNumber,
            Id = application.Id,
        };

        return Result.Ok(response);
    }
}