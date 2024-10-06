using System.Text.Json.Serialization;
using SmartCoinOS.Domain.Application.Enums;
using SmartCoinOS.Domain.SeedWork;
using SmartCoinOS.Domain.Shared;

namespace SmartCoinOS.Domain.Application;

public sealed class BusinessInfoForm : ApplicationForm
{
    internal BusinessInfoForm() { }

    [JsonConstructor]
    internal BusinessInfoForm(
        SourcesOfWealthRecord sourcesOfWealth,
        PoliticallyExposedPersonRecord politicallyExposedPerson,
        LinesOfBusinessRecord linesOfBusiness,
        BusinessActivitiesRecord businessActivities,
        ProjectedOrdersRecord projectedOrders)
    {
        SourcesOfWealth = sourcesOfWealth;
        PoliticallyExposedPerson = politicallyExposedPerson;
        LinesOfBusiness = linesOfBusiness;
        BusinessActivities = businessActivities;
        ProjectedOrders = projectedOrders;
    }

    public required SourcesOfWealthRecord SourcesOfWealth { get; init; }
    public required PoliticallyExposedPersonRecord PoliticallyExposedPerson { get; init; }
    public required LinesOfBusinessRecord LinesOfBusiness { get; init; }
    public required BusinessActivitiesRecord BusinessActivities { get; init; }
    public required ProjectedOrdersRecord ProjectedOrders { get; init; }

    internal override bool IsVerified()
    {
        return SourcesOfWealth.IsVerified()
               && PoliticallyExposedPerson.IsVerified()
               && LinesOfBusiness.IsVerified()
               && BusinessActivities.IsVerified()
               && ProjectedOrders.IsVerified();
    }

    internal override void RequireAdditionalInfo()
    {
        if (!IsVerified())
            return;

        SourcesOfWealth.RemoveVerificationFlag();
        PoliticallyExposedPerson.RemoveVerificationFlag();
        LinesOfBusiness.RemoveVerificationFlag();
        BusinessActivities.RemoveVerificationFlag();
        ProjectedOrders.RemoveVerificationFlag();
    }

    internal bool RemoveDocument(DocumentId documentId)
    {
        var foundDocument = SourcesOfWealth.Documents.Find(x => x.DocumentId == documentId);
        if (foundDocument is null)
            return false;

        var removed = SourcesOfWealth.Documents.Remove(foundDocument);
        if (removed)
            RequireAdditionalInfo();

        return removed;
    }

    internal override Result Verify(string segmentOrRecord, UserInfo userInfo)
    {
        return segmentOrRecord switch
        {
            BusinessInfoRecords.SourceOfWealth => SourcesOfWealth.Verify(userInfo),
            BusinessInfoRecords.PoliticallyExposedPerson => PoliticallyExposedPerson.Verify(userInfo),
            BusinessInfoRecords.LinesOfBusiness => LinesOfBusiness.Verify(userInfo),
            BusinessInfoRecords.BusinessActivities => BusinessActivities.Verify(userInfo),
            BusinessInfoRecords.ProjectedOrders => ProjectedOrders.Verify(userInfo),

            _ => throw new InvalidOperationException()
        };
    }
}

public sealed record SourcesOfWealthRecord : VerifiableRecord
{
    [JsonConstructor]
    internal SourcesOfWealthRecord(
        bool contributionsOfMembersOrShareHolders,
        bool capitalGainsOrDividends,
        bool financialInstrumentOrInvestments,
        bool incomeFromSaleOfAssets,
        bool settlementOfAccountsOrServicePayments,
        bool loan,
        bool otherSources,
        string? otherSourcesDetails,
        string furtherInformation,
        List<ApplicationDocument> documents,
        UserInfo? verifiedBy = null,
        DateTimeOffset? verifiedAt = null) : base(verifiedBy, verifiedAt)
    {
        ContributionsOfMembersOrShareHolders = contributionsOfMembersOrShareHolders;
        CapitalGainsOrDividends = capitalGainsOrDividends;
        FinancialInstrumentOrInvestments = financialInstrumentOrInvestments;
        IncomeFromSaleOfAssets = incomeFromSaleOfAssets;
        SettlementOfAccountsOrServicePayments = settlementOfAccountsOrServicePayments;
        Loan = loan;
        OtherSources = otherSources;
        OtherSourcesDetails = otherSourcesDetails;
        FurtherInformation = furtherInformation;
        Documents = documents;
    }

    public bool ContributionsOfMembersOrShareHolders { get; }
    public bool CapitalGainsOrDividends { get; }
    public bool FinancialInstrumentOrInvestments { get; }
    public bool IncomeFromSaleOfAssets { get; }
    public bool SettlementOfAccountsOrServicePayments { get; }
    public bool Loan { get; }
    public bool OtherSources { get; }
    public string? OtherSourcesDetails { get; }
    public string FurtherInformation { get; }
    public List<ApplicationDocument> Documents { get; } = [];
}

public sealed record PoliticallyExposedPersonRecord : VerifiableRecord
{
    [JsonConstructor]
    internal PoliticallyExposedPersonRecord(
        bool isPep, string? pepDetails,
        bool isFamilyMemberPep, string? familyMemberPepDetails,
        bool isCloseAssociatePep, string? closeAssociatePepDetails,
        UserInfo? verifiedBy = null,
        DateTimeOffset? verifiedAt = null) : base(verifiedBy, verifiedAt)
    {
        IsPep = isPep;
        PepDetails = pepDetails;
        IsFamilyMemberPep = isFamilyMemberPep;
        FamilyMemberPepDetails = familyMemberPepDetails;
        IsCloseAssociatePep = isCloseAssociatePep;
        CloseAssociatePepDetails = closeAssociatePepDetails;
    }

    public bool IsPep { get; }
    public string? PepDetails { get; }

    public bool IsFamilyMemberPep { get; }
    public string? FamilyMemberPepDetails { get; }

    public bool IsCloseAssociatePep { get; }
    public string? CloseAssociatePepDetails { get; }
}

public sealed record LinesOfBusinessRecord : VerifiableRecord
{
    [JsonConstructor]
    internal LinesOfBusinessRecord(
        BusinessEntityType businessEntityType,
        bool accountingOrFinancialServices, bool administrativeAndSupportServiceActivities,
        bool consultingOrProfessionalServices, bool consumerGoodsAndServices,
        bool eCommerce, bool educationInstitutionOrServices,
        bool gambling, bool gamingAndEsports, bool government,
        bool hotelsRestaurantsOrLeisure, bool humanResourcesOrEmploymentServices,
        bool managementOfOwnAssets, bool materialsManufacturingAndConstruction,
        bool nonProfitOrganization, bool professionalScientificAndTechnicalActivities,
        bool realEstateActivities, bool travelTransportationOrAutomotive,
        bool softwareItOrHardware, bool mediaAndAdvertising,
        bool otherSources, string? otherSourcesDetails,
        UserInfo? verifiedBy = null,
        DateTimeOffset? verifiedAt = null) : base(verifiedBy, verifiedAt)
    {
        BusinessEntityType = businessEntityType;
        AccountingOrFinancialServices = accountingOrFinancialServices;
        AdministrativeAndSupportServiceActivities = administrativeAndSupportServiceActivities;
        ConsultingOrProfessionalServices = consultingOrProfessionalServices;
        ConsumerGoodsAndServices = consumerGoodsAndServices;
        ECommerce = eCommerce;
        EducationInstitutionOrServices = educationInstitutionOrServices;
        Gambling = gambling;
        GamingAndEsports = gamingAndEsports;
        Government = government;
        HotelsRestaurantsOrLeisure = hotelsRestaurantsOrLeisure;
        HumanResourcesOrEmploymentServices = humanResourcesOrEmploymentServices;
        ManagementOfOwnAssets = managementOfOwnAssets;
        MaterialsManufacturingAndConstruction = materialsManufacturingAndConstruction;
        NonProfitOrganization = nonProfitOrganization;
        ProfessionalScientificAndTechnicalActivities = professionalScientificAndTechnicalActivities;
        RealEstateActivities = realEstateActivities;
        TravelTransportationOrAutomotive = travelTransportationOrAutomotive;
        SoftwareItOrHardware = softwareItOrHardware;
        MediaAndAdvertising = mediaAndAdvertising;
        OtherSources = otherSources;
        OtherSourcesDetails = otherSourcesDetails;
    }

    public BusinessEntityType BusinessEntityType { get; }

    public bool AccountingOrFinancialServices { get; }
    public bool AdministrativeAndSupportServiceActivities { get; }
    public bool ConsultingOrProfessionalServices { get; }
    public bool ConsumerGoodsAndServices { get; }
    public bool ECommerce { get; }
    public bool EducationInstitutionOrServices { get; }
    public bool Gambling { get; }
    public bool GamingAndEsports { get; }
    public bool Government { get; }
    public bool HotelsRestaurantsOrLeisure { get; }
    public bool HumanResourcesOrEmploymentServices { get; }
    public bool ManagementOfOwnAssets { get; }
    public bool MaterialsManufacturingAndConstruction { get; }
    public bool NonProfitOrganization { get; }
    public bool ProfessionalScientificAndTechnicalActivities { get; }
    public bool RealEstateActivities { get; }
    public bool TravelTransportationOrAutomotive { get; }
    public bool SoftwareItOrHardware { get; }
    public bool MediaAndAdvertising { get; }
    public bool OtherSources { get; }
    public string? OtherSourcesDetails { get; set; }
}

public sealed record BusinessActivitiesRecord : VerifiableRecord
{
    [JsonConstructor]
    internal BusinessActivitiesRecord(
        string description,
        bool isFinancialStatementsRequired, string? financialStatementsDetails,
        bool hasHongKongBusinessActivities, string? hongKongBusinessActivitiesDetails,
        bool isTaxResidentInCorpOriginCountry, string? taxResidentInCorpOriginCountryDetails,
        bool hasTaxResidentInOtherCountries, string? otherTaxResidencyCountries,
        string? otherCountryTins, bool isSpecialPermissionRequired,
        string? specialPermissionDetails, ApproximateNumberOfEmployees approximateNumberOfEmployees,
        decimal annualNetTurnover,
        UserInfo? verifiedBy = null,
        DateTimeOffset? verifiedAt = null) : base(verifiedBy, verifiedAt)
    {
        Description = description;
        IsFinancialStatementsRequired = isFinancialStatementsRequired;
        FinancialStatementsDetails = financialStatementsDetails;
        HasHongKongBusinessActivities = hasHongKongBusinessActivities;
        HongKongBusinessActivitiesDetails = hongKongBusinessActivitiesDetails;
        IsTaxResidentInCorpOriginCountry = isTaxResidentInCorpOriginCountry;
        TaxResidentInCorpOriginCountryDetails = taxResidentInCorpOriginCountryDetails;
        HasTaxResidentInOtherCountries = hasTaxResidentInOtherCountries;
        OtherTaxResidencyCountries = otherTaxResidencyCountries;
        OtherCountryTins = otherCountryTins;
        IsSpecialPermissionRequired = isSpecialPermissionRequired;
        SpecialPermissionDetails = specialPermissionDetails;
        ApproximateNumberOfEmployees = approximateNumberOfEmployees;
        AnnualNetTurnover = annualNetTurnover;
    }

    public string Description { get; }

    public bool IsFinancialStatementsRequired { get; }
    public string? FinancialStatementsDetails { get; }

    public bool HasHongKongBusinessActivities { get; }
    public string? HongKongBusinessActivitiesDetails { get; }

    public bool IsTaxResidentInCorpOriginCountry { get; }
    public string? TaxResidentInCorpOriginCountryDetails { get; }

    public bool HasTaxResidentInOtherCountries { get; }
    public string? OtherTaxResidencyCountries { get; }

    /// <summary>
    /// Other countries tax payer identification numbers
    /// </summary>
    public string? OtherCountryTins { get; }

    public bool IsSpecialPermissionRequired { get; }
    public string? SpecialPermissionDetails { get; }

    public ApproximateNumberOfEmployees ApproximateNumberOfEmployees { get; }
    public decimal AnnualNetTurnover { get; }
}

public sealed record ProjectedOrdersRecord : VerifiableRecord
{
    [JsonConstructor]
    internal ProjectedOrdersRecord(
        StableCoinsMintedAmount expectedMonthlyStableCoinsMintedAmount,
        CumulativeStableCoinsMints expectedMonthlyCumulativeMints,
        double maxAnticipatedAmountForSingleMint,
        int expectedIrregularMintsFirstYear,
        UserInfo? verifiedBy = null,
        DateTimeOffset? verifiedAt = null) : base(verifiedBy, verifiedAt)
    {
        ExpectedMonthlyStableCoinsMintedAmount = expectedMonthlyStableCoinsMintedAmount;
        ExpectedMonthlyCumulativeMints = expectedMonthlyCumulativeMints;
        MaxAnticipatedAmountForSingleMint = maxAnticipatedAmountForSingleMint;
        ExpectedIrregularMintsFirstYear = expectedIrregularMintsFirstYear;
    }

    public StableCoinsMintedAmount ExpectedMonthlyStableCoinsMintedAmount { get; }
    public CumulativeStableCoinsMints ExpectedMonthlyCumulativeMints { get; }
    public double MaxAnticipatedAmountForSingleMint { get; }
    public int ExpectedIrregularMintsFirstYear { get; }
}
