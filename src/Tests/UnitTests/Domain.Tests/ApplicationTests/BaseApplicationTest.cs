using NSubstitute;
using SmartCoinOS.Domain.Application;
using SmartCoinOS.Domain.Application.Enums;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Enums;
using SmartCoinOS.Domain.SeedWork;
using SmartCoinOS.Domain.Shared;

namespace SmartCoinOS.Tests.Domain.ApplicationTests;

public abstract class BaseApplicationTest
{
    protected const string LegalEntityName = "JPMorgan Chase";

    protected static readonly UserInfo UserInfo = new()
    {
        Id = Guid.NewGuid(), Type = UserInfoType.BackOffice, Username = "Angela Smith"
    };

    protected static readonly ApplicationDocument DefaultApplicationDocument = new()
    {
        DocumentType = "document-type",
        DocumentId = DocumentId.Parse("0525d6f8-a988-4505-bf1a-e6aae7145482"),
        FileName = "file-name.pdf"
    };

    protected static readonly AddressRecord SampleAddress = new()
    {
        Country = "United Kingdom", City = "West Lornaport", PostalCode = "97048", Street = "1121 1st street"
    };

    private static IApplicationNumberService MockApplicationNumberService()
    {
        var applicationNumber = ApplicationNumber.New();

        var applicationNumberService = Substitute.For<IApplicationNumberService>();
        applicationNumberService.GetApplicationNumberAsync(CancellationToken.None).Returns(applicationNumber);

        return applicationNumberService;
    }

    protected static Application GetApplicationFromFactory()
    {
        var applicationService = new ApplicationService(MockApplicationNumberService());
        var application = applicationService.CreateAsync(LegalEntityName, UserInfo, CancellationToken.None).Result;

        return application;
    }

    protected static Application GetApplication()
    {
        var application = GetApplicationWithForms();

        return application;
    }

    protected static Application GetApplicationInReview()
    {
        var application = GetApplicationWithForms();
        VerifyAllForms(application);
        application.Submit(UserInfo);

        return application;
    }

    private static Application GetApplicationWithForms()
    {
        var application = GetApplicationFromFactory();

        var businessInfo = BuildBusinessInfoForm();
        application.SetBusinessInfo(businessInfo);

        application.SetEntityParticulars(legalStructure: CompanyLegalStructure.Other,
            structureDetails: "Lorem Ipsum is simply dummy text of the printing and typesetting industry",
            registrationNumber: "123456789",
            dateOfInc: DateOnly.FromDateTime(DateTime.UtcNow),
            countryOfInc: "USA",
            documents: [DefaultApplicationDocument],
            street: "Lorem Ipsum is simply dummy text",
            postalCode: "10003",
            state: "NY",
            country: "USA",
            city: "New York",
            email: "email@gmail.com",
            phone: "0356565698",
            userInfo: UserInfo);

        application.SetAuthorizedUsersForm(firstName: "Join",
            lastName: "Smith",
            email: "userid",
            personalVerification: DefaultApplicationDocument,
            proofOfAddress: DefaultApplicationDocument,
            userInfo: UserInfo);

        application.SetBankAccounts(beneficiary: "beneficiary",
            bankName: "bankName",
            iban: "iban",
            swift: "swift",
            new SmartTrustBank(12, 122),
            SampleAddress,
            documents:
            [DefaultApplicationDocument],
            userInfo:
            UserInfo);

        application.SetWallets(network: BlockchainNetwork.BnbSmartchain, address: "walletAddress");

        application.SetFdtAccounts(clientName: "clientId", accountNumber: "accountNumber");
        var sampleDocumentRecord = new OnboardingDocumentRecord
        {
            MemorandumOfAssociation = DefaultApplicationDocument,
            BankStatement = DefaultApplicationDocument,
            CertificateOfIncumbency = DefaultApplicationDocument,
            RegisterOfDirectors = DefaultApplicationDocument,
            RegisterOfShareholder = DefaultApplicationDocument,
            GroupOwnershipAndStructure = DefaultApplicationDocument,
            BoardResolutionAppointingAdditionalUsers = DefaultApplicationDocument,
            CertificateOfIncAndNameChange = DefaultApplicationDocument,
            AdditionalDocuments = [DefaultApplicationDocument],
            CreatedAt = DateTimeOffset.UtcNow
        };

        application.SetDocuments(sampleDocumentRecord);

        return application;
    }

    private static BusinessInfoForm BuildBusinessInfoForm()
    {
        var businessInfoForm = new BusinessInfoFormBuilder();

        businessInfoForm.AddSourcesOfWealth(contributionsOfMembersOrShareHolders: true, capitalGainsOrDividends: true,
            financialInstrumentOrInvestments: true, incomeFromSaleOfAssets: true,
            settlementOfAccountsOrServicePayments: true, loan: true, otherSources: true, "otherSourcesDetails",
            "furtherInformation", documents: [DefaultApplicationDocument]);

        businessInfoForm.AddPoliticallyExposedPerson(isPep: true, "pepDetails", isFamilyMemberPep: true,
            "familyMemberPepDetails", isCloseAssociatePep: true, "closeAssociatePepDetails");

        businessInfoForm.AddLinesOfBusiness(BusinessEntityType.IndependentEntity, accountingOrFinancialServices: true,
            administrativeAndSupportServiceActivities: true, consultingOrProfessionalServices: true,
            consumerGoodsAndServices: true, eCommerce: true, educationInstitutionOrServices: true, gambling: true,
            gamingAndEsports: true, government: true, hotelsRestaurantsOrLeisure: true,
            humanResourcesOrEmploymentServices: true, managementOfOwnAssets: true,
            materialsManufacturingAndConstruction: true, nonProfitOrganization: true,
            professionalScientificAndTechnicalActivities: true, realEstateActivities: true,
            travelTransportationOrAutomotive: true, softwareItOrHardware: true, mediaAndAdvertising: true,
            otherSources: true, "otherSourcesDetails");

        businessInfoForm.AddBusinessActivities("description", isFinancialStatementsRequired: true,
            "financialStatementsDetails", hasHongKongBusinessActivities: true, "hongKongBusinessActivitiesDetails",
            isTaxResidentInCorpOriginCountry: true, "taxResidentInCorpOriginCountryDetails",
            hasTaxResidentInOtherCountries: true, "otherTaxResidencyCountries", "otherCountryTins",
            isSpecialPermissionRequired: true, "specialPermissionDetails",
            approximateNumberOfEmployees: ApproximateNumberOfEmployees.ZeroToNine, annualNetTurnover: 2);

        businessInfoForm.AddProjectedOrders(
            expectedMonthlyStableCoinsMintedAmount: StableCoinsMintedAmount.FiftyMillionsAndMore,
            expectedMonthlyCumulativeMints: CumulativeStableCoinsMints.ZeroToNine, maxAnticipatedAmountForSingleMint: 9,
            expectedIrregularMintsFirstYear: 23);

        return businessInfoForm.Build();
    }

    private static void VerifyAllForms(Application application)
    {
        application.VerifyFormRecord(ApplicationFormType.EntityParticulars, EntityParticularRecords.CompanyParticulars,
            UserInfo);
        application.VerifyFormRecord(ApplicationFormType.EntityParticulars, EntityParticularRecords.RegistrationAddress,
            UserInfo);
        application.VerifyFormRecord(ApplicationFormType.EntityParticulars, EntityParticularRecords.ContactForm,
            UserInfo);

        application.VerifyFormRecord(ApplicationFormType.BusinessInfo, BusinessInfoRecords.SourceOfWealth, UserInfo);
        application.VerifyFormRecord(ApplicationFormType.BusinessInfo, BusinessInfoRecords.PoliticallyExposedPerson,
            UserInfo);
        application.VerifyFormRecord(ApplicationFormType.BusinessInfo, BusinessInfoRecords.LinesOfBusiness, UserInfo);
        application.VerifyFormRecord(ApplicationFormType.BusinessInfo, BusinessInfoRecords.BusinessActivities,
            UserInfo);
        application.VerifyFormRecord(ApplicationFormType.BusinessInfo, BusinessInfoRecords.ProjectedOrders, UserInfo);

        application.AuthorizedUsers.AuthorizedUsers.ForEach(x =>
            application.VerifyFormRecord(ApplicationFormType.AuthorizedUsers, x.Id.ToString(), UserInfo));
        application.BankAccounts.BankRecords.ForEach(x =>
            application.VerifyFormRecord(ApplicationFormType.BankAccounts, x.Id.ToString(), UserInfo));
        application.Wallets.Wallets.ForEach(x =>
            application.VerifyFormRecord(ApplicationFormType.Wallets, x.Id.ToString(), UserInfo));
        application.FdtAccounts.FdtAccounts.ForEach(x =>
            application.VerifyFormRecord(ApplicationFormType.FdtAccounts, x.Id.ToString(), UserInfo));
        application.VerifyFormRecord(ApplicationFormType.Documents, DocumentsRecords.DueDiligenceRecords, UserInfo);
    }
}
