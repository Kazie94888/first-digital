using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Application.Enums;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.BusinessInfo;

public sealed record SetBusinessInfoFormCommand : ICommand<ApplicationFormMetadataDto>
{
    [FromRoute(Name = "applicationId")]
    public required ApplicationId ApplicationId { get; init; }

    [FromBody]
    public required SetBusinessInfoRequest BusinessInfo { get; init; }
}

public sealed record SetBusinessInfoRequest
{
    public required SourcesOfWealthRequest SourcesOfWealth { get; init; }
    public required PoliticallyExposedPersonRequest PoliticallyExposedPerson { get; init; }
    public required LinesOfBusinessRequest LinesOfBusiness { get; init; }
    public required BusinessActivitiesRequest BusinessActivities { get; init; }
    public required ProjectedOrdersRequest ProjectedOrders { get; init; }
}

public sealed record SourcesOfWealthRequest
{
    public required bool ContributionsOfMembersOrShareHolders { get; init; }
    public required bool CapitalGainsOrDividends { get; init; }
    public required bool FinancialInstrumentOrInvestments { get; init; }
    public required bool IncomeFromSaleOfAssets { get; init; }
    public required bool SettlementOfAccountsOrServicePayments { get; init; }
    public required bool Loan { get; init; }
    public required bool OtherSources { get; init; }
    public string? OtherSourcesDetails { get; init; }
    public required string FurtherInformation { get; init; }
    public required List<DocumentDto> Documents { get; init; }
}

public sealed record PoliticallyExposedPersonRequest
{
    public required bool IsPep { get; init; }
    public string? PepDetails { get; init; }
    public required bool IsFamilyMemberPep { get; init; }
    public string? FamilyMemberPepDetails { get; init; }
    public required bool IsCloseAssociatePep { get; init; }
    public string? CloseAssociatePepDetails { get; init; }
}

public sealed record LinesOfBusinessRequest
{
    public required BusinessEntityType BusinessEntityType { get; init; }
    public required bool AccountingOrFinancialServices { get; init; }
    public required bool AdministrativeAndSupportServiceActivities { get; init; }
    public required bool ConsultingOrProfessionalServices { get; init; }
    public required bool ConsumerGoodsAndServices { get; init; }
    public required bool ECommerce { get; init; }
    public required bool EducationInstitutionOrServices { get; init; }
    public required bool Gambling { get; init; }
    public required bool GamingAndEsports { get; init; }
    public required bool Government { get; init; }
    public required bool HotelsRestaurantsOrLeisure { get; init; }
    public required bool HumanResourcesOrEmploymentServices { get; init; }
    public required bool ManagementOfOwnAssets { get; init; }
    public required bool MaterialsManufacturingAndConstruction { get; init; }
    public required bool NonProfitOrganization { get; init; }
    public required bool ProfessionalScientificAndTechnicalActivities { get; init; }
    public required bool RealEstateActivities { get; init; }
    public required bool TravelTransportationOrAutomotive { get; init; }
    public required bool SoftwareItOrHardware { get; init; }
    public required bool MediaAndAdvertising { get; init; }
    public required bool OtherSources { get; init; }
    public string? OtherSourcesDetails { get; init; }
}

public sealed record BusinessActivitiesRequest
{
    public required string Description { get; init; }
    public required bool IsFinancialStatementsRequired { get; init; }
    public string? FinancialStatementsDetails { get; init; }
    public required bool HasHongKongBusinessActivities { get; init; }
    public string? HongKongBusinessActivitiesDetails { get; init; }
    public required bool IsTaxResidentInCorpOriginCountry { get; init; }
    public string? TaxResidentInCorpOriginCountryDetails { get; init; }
    public required bool HasTaxResidentInOtherCountries { get; init; }
    public string? OtherTaxResidencyCountries { get; init; }
    public string? OtherCountryTins { get; init; }
    public required bool IsSpecialPermissionRequired { get; init; }
    public string? SpecialPermissionDetails { get; init; }
    public required ApproximateNumberOfEmployees ApproximateNumberOfEmployees { get; init; }
    public required decimal AnnualNetTurnover { get; init; }
}

public sealed record ProjectedOrdersRequest
{
    public required StableCoinsMintedAmount ExpectedMonthlyStableCoinsMintedAmount { get; init; }
    public required CumulativeStableCoinsMints ExpectedMonthlyCumulativeMints { get; init; }
    public required double MaxAnticipatedAmountForSingleMint { get; init; }
    public required int ExpectedIrregularMintsFirstYear { get; init; }
}