using SmartCoinOS.Domain.Application.Enums;
using SmartCoinOS.Domain.Exceptions;

namespace SmartCoinOS.Domain.Application;
public sealed class BusinessInfoFormBuilder
{
    private SourcesOfWealthRecord? _sourcesOfWealth;
    private PoliticallyExposedPersonRecord? _politicallyExposedPerson;
    private LinesOfBusinessRecord? _linesOfBusiness;
    private BusinessActivitiesRecord? _businessActivities;
    private ProjectedOrdersRecord? _projectedOrders;


    public BusinessInfoFormBuilder AddSourcesOfWealth(bool contributionsOfMembersOrShareHolders,
        bool capitalGainsOrDividends, bool financialInstrumentOrInvestments, bool incomeFromSaleOfAssets,
        bool settlementOfAccountsOrServicePayments, bool loan, bool otherSources, string? otherSourcesDetails,
        string furtherInformation, List<ApplicationDocument> documents)
    {
        _sourcesOfWealth = new SourcesOfWealthRecord(contributionsOfMembersOrShareHolders,
                    capitalGainsOrDividends,
                    financialInstrumentOrInvestments,
                    incomeFromSaleOfAssets,
                    settlementOfAccountsOrServicePayments,
                    loan,
                    otherSources,
                    otherSourcesDetails,
                    furtherInformation,
                    documents);

        return this;
    }

    public BusinessInfoFormBuilder AddPoliticallyExposedPerson(bool isPep, string? pepDetails, bool isFamilyMemberPep,
        string? familyMemberPepDetails, bool isCloseAssociatePep, string? closeAssociatePepDetails)
    {
        _politicallyExposedPerson = new PoliticallyExposedPersonRecord(isPep, pepDetails, isFamilyMemberPep,
                                                                       familyMemberPepDetails, isCloseAssociatePep,
                                                                       closeAssociatePepDetails);

        return this;
    }

    public BusinessInfoFormBuilder AddLinesOfBusiness(
        BusinessEntityType businessEntityType, bool accountingOrFinancialServices,
        bool administrativeAndSupportServiceActivities, bool consultingOrProfessionalServices,
        bool consumerGoodsAndServices, bool eCommerce, bool educationInstitutionOrServices, bool gambling,
        bool gamingAndEsports, bool government, bool hotelsRestaurantsOrLeisure, bool humanResourcesOrEmploymentServices,
        bool managementOfOwnAssets, bool materialsManufacturingAndConstruction, bool nonProfitOrganization,
        bool professionalScientificAndTechnicalActivities, bool realEstateActivities,
        bool travelTransportationOrAutomotive, bool softwareItOrHardware, bool mediaAndAdvertising, bool otherSources,
        string? otherSourcesDetails)
    {
        _linesOfBusiness = new LinesOfBusinessRecord(businessEntityType, accountingOrFinancialServices,
                                                     administrativeAndSupportServiceActivities,
                                                     consultingOrProfessionalServices, consumerGoodsAndServices,
                                                     eCommerce, educationInstitutionOrServices, gambling,
                                                     gamingAndEsports, government, hotelsRestaurantsOrLeisure,
                                                     humanResourcesOrEmploymentServices, managementOfOwnAssets,
                                                     materialsManufacturingAndConstruction, nonProfitOrganization,
                                                     professionalScientificAndTechnicalActivities, realEstateActivities,
                                                     travelTransportationOrAutomotive, softwareItOrHardware,
                                                     mediaAndAdvertising, otherSources, otherSourcesDetails);

        return this;
    }

    public BusinessInfoFormBuilder AddBusinessActivities(string description, bool isFinancialStatementsRequired,
        string? financialStatementsDetails, bool hasHongKongBusinessActivities,
        string? hongKongBusinessActivitiesDetails, bool isTaxResidentInCorpOriginCountry,
        string? taxResidentInCorpOriginCountryDetails, bool hasTaxResidentInOtherCountries,
        string? otherTaxResidencyCountries, string? otherCountryTins, bool isSpecialPermissionRequired,
        string? specialPermissionDetails, ApproximateNumberOfEmployees approximateNumberOfEmployees,
        decimal annualNetTurnover)
    {
        _businessActivities = new BusinessActivitiesRecord(description, isFinancialStatementsRequired,
            financialStatementsDetails, hasHongKongBusinessActivities, hongKongBusinessActivitiesDetails,
            isTaxResidentInCorpOriginCountry, taxResidentInCorpOriginCountryDetails, hasTaxResidentInOtherCountries,
            otherTaxResidencyCountries, otherCountryTins, isSpecialPermissionRequired, specialPermissionDetails,
            approximateNumberOfEmployees, annualNetTurnover);

        return this;
    }

    public BusinessInfoFormBuilder AddProjectedOrders(
        StableCoinsMintedAmount expectedMonthlyStableCoinsMintedAmount,
        CumulativeStableCoinsMints expectedMonthlyCumulativeMints,
        double maxAnticipatedAmountForSingleMint,
        int expectedIrregularMintsFirstYear)
    {
        _projectedOrders = new ProjectedOrdersRecord(
            expectedMonthlyStableCoinsMintedAmount,
            expectedMonthlyCumulativeMints,
            maxAnticipatedAmountForSingleMint,
            expectedIrregularMintsFirstYear);

        return this;
    }

    public BusinessInfoForm Build()
    {
        if (_sourcesOfWealth is null || _politicallyExposedPerson is null
            || _linesOfBusiness is null || _businessActivities is null || _projectedOrders is null)
            throw new EntityInvalidStateException("Cannot build a business info form because one or more verifiable records are missing.");

        return new BusinessInfoForm
        {
            SourcesOfWealth = _sourcesOfWealth,
            BusinessActivities = _businessActivities,
            LinesOfBusiness = _linesOfBusiness,
            PoliticallyExposedPerson = _politicallyExposedPerson,
            ProjectedOrders = _projectedOrders
        };
    }
}
