using Bogus;
using SmartCoinOS.Domain.Application;
using SmartCoinOS.Domain.Application.Enums;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Enums;
using SmartCoinOS.Domain.SeedWork;
using SmartCoinOS.Domain.Shared;
using OnboardingApplication = SmartCoinOS.Domain.Application.Application;

namespace SmartCoinOS.SmartCoinTool.DataSeed;

internal static class ApplicationSeedProvider
{
    private static readonly DateTimeOffset _referenceDate = new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);

    private static readonly UserInfo _userInfo = new()
    {
        Id = Guid.NewGuid(),
        Type = UserInfoType.BackOffice,
        Username = "Seeder"
    };

    private const int _seedValue = 12345;

    static ApplicationSeedProvider()
    {
        Randomizer.Seed = new Random(_seedValue);
    }

    private static Faker<OnboardingApplication> GetOnboardingApplicationFaker()
    {
        return new Faker<OnboardingApplication>().CustomInstantiator(f =>
        {
            var application = OnboardingApplication.Create(f.Company.CompanyName(), ApplicationNumber.New(), _userInfo);
            SetEntityParticulars(application, f);
            SetAuthorizedUsersForm(application, f);
            SetBusinessInfo(application, f);
            SetBankAccounts(application, f);
            SetWallets(application, f);
            SetFdtAccounts(application, f);
            SetDocuments(application, f);

            return application;
        });
    }

    private static void SetDocuments(OnboardingApplication application, Faker f)
    {
        var documentForm = new OnboardingDocumentRecord()
        {
            MemorandumOfAssociation = GetDocument("memorandum-of-association", f.System.FileName()),
            BankStatement = GetDocument("bank-statement", f.System.FileName()),
            CertificateOfIncumbency = GetDocument("certificate-of-incumbency", f.System.FileName()),
            GroupOwnershipAndStructure = GetDocument("group-ownership-and-structure", f.System.FileName()),
            CertificateOfIncAndNameChange = GetDocument("certificate-of-inc-and-name-change", f.System.FileName()),
            RegisterOfShareholder = GetDocument("register-of-shareholder", f.System.FileName()),
            RegisterOfDirectors = GetDocument("register-of-directors", f.System.FileName()),
            BoardResolutionAppointingAdditionalUsers =
                GetDocument("board-resolution-appointing-additional-users", f.System.FileName()),
            AdditionalDocuments = []
        };

        application.SetDocuments(documentForm);
        application.Documents?.OnboardingDocument.Verify(_userInfo);
    }

    private static ApplicationDocument GetDocument(string fileName, string documentType)
    {
        return new ApplicationDocument
        {
            DocumentType = documentType,
            DocumentId = DocumentId.New(),
            FileName = fileName
        };
    }

    private static void SetFdtAccounts(OnboardingApplication application, Faker f)
    {
        application.SetFdtAccounts(f.Company.CompanyName(), f.Company.Random.AlphaNumeric(10));
        application.FdtAccounts.FdtAccounts.ForEach(x => x.Verify(_userInfo));
    }

    private static void SetWallets(OnboardingApplication application, Faker f)
    {
        application.SetWallets(f.PickRandom<BlockchainNetwork>(), f.Finance.EthereumAddress());
        application.SetWallets(f.PickRandom<BlockchainNetwork>(), f.Finance.EthereumAddress());
        application.Wallets.Wallets.ForEach(x => x.Verify(_userInfo));
    }

    private static void SetBankAccounts(OnboardingApplication application, Faker f)
    {
        // this is a known value. if we add here a random number, then it will not be possible to create orders from this bank
        var smartTrustBank = new SmartTrustBank(9690, 9690);

        var address = new AddressRecord()
        {
            Country = DataSeedProvider.GetRandomCountry(),
            City = f.Address.City(),
            Street = f.Address.StreetAddress(),
            PostalCode = f.Address.ZipCode(),
            State = f.Address.State(),
        };

        application.SetBankAccounts(f.Name.FullName(), f.Company.CompanyName(), f.Finance.Iban(), f.Finance.Bic(),
            smartTrustBank,
            address,
            [
                GetDocument("bank-statement-1.pdf", "bank-statement-1"),
                GetDocument("bank-statement-2.pdf", "bank-statement-2")
            ], _userInfo);
        application.BankAccounts.BankRecords.ForEach(x => x.Verify(_userInfo));
    }

    private static void SetBusinessInfo(OnboardingApplication application, Faker f)
    {
        var builder = new BusinessInfoFormBuilder();

        builder.AddSourcesOfWealth(
            contributionsOfMembersOrShareHolders: f.Random.Bool(),
            capitalGainsOrDividends: f.Random.Bool(),
            financialInstrumentOrInvestments: f.Random.Bool(),
            incomeFromSaleOfAssets: f.Random.Bool(),
            settlementOfAccountsOrServicePayments: f.Random.Bool(),
            loan: f.Random.Bool(),
            otherSources: f.Random.Bool(),
            otherSourcesDetails: f.Lorem.Sentence(),
            furtherInformation: f.Lorem.Sentence(),
            documents: [GetDocument("source-of-wealth.pdf", "source-of-wealth")]
        );

        builder.AddPoliticallyExposedPerson(
            isPep: f.Random.Bool(),
            pepDetails: f.Lorem.Sentence(),
            isFamilyMemberPep: f.Random.Bool(),
            familyMemberPepDetails: f.Lorem.Sentence(),
            isCloseAssociatePep: f.Random.Bool(),
            closeAssociatePepDetails: f.Lorem.Sentence()
        );

        builder.AddLinesOfBusiness(
            businessEntityType: f.PickRandom<BusinessEntityType>(),
            accountingOrFinancialServices: f.Random.Bool(),
            administrativeAndSupportServiceActivities: f.Random.Bool(),
            consultingOrProfessionalServices: f.Random.Bool(),
            consumerGoodsAndServices: f.Random.Bool(),
            eCommerce: f.Random.Bool(),
            educationInstitutionOrServices: f.Random.Bool(),
            gambling: f.Random.Bool(),
            gamingAndEsports: f.Random.Bool(),
            government: f.Random.Bool(),
            hotelsRestaurantsOrLeisure: f.Random.Bool(),
            humanResourcesOrEmploymentServices: f.Random.Bool(),
            managementOfOwnAssets: f.Random.Bool(),
            materialsManufacturingAndConstruction: f.Random.Bool(),
            nonProfitOrganization: f.Random.Bool(),
            professionalScientificAndTechnicalActivities: f.Random.Bool(),
            realEstateActivities: f.Random.Bool(),
            travelTransportationOrAutomotive: f.Random.Bool(),
            softwareItOrHardware: f.Random.Bool(),
            mediaAndAdvertising: f.Random.Bool(),
            otherSources: f.Random.Bool(),
            otherSourcesDetails: f.Lorem.Sentence()
        );

        builder.AddBusinessActivities(
            description: f.Lorem.Sentence(),
            isFinancialStatementsRequired: f.Random.Bool(),
            financialStatementsDetails: f.Lorem.Sentence(),
            hasHongKongBusinessActivities: f.Random.Bool(),
            hongKongBusinessActivitiesDetails: f.Lorem.Sentence(),
            isTaxResidentInCorpOriginCountry: f.Random.Bool(),
            taxResidentInCorpOriginCountryDetails: f.Lorem.Sentence(),
            hasTaxResidentInOtherCountries: f.Random.Bool(),
            otherTaxResidencyCountries: f.Lorem.Sentence(),
            otherCountryTins: f.Lorem.Sentence(),
            isSpecialPermissionRequired: f.Random.Bool(),
            specialPermissionDetails: f.Lorem.Sentence(),
            approximateNumberOfEmployees: f.PickRandom<ApproximateNumberOfEmployees>(),
            annualNetTurnover: f.Random.Decimal(1_000_000, 100_000_000)
        );

        builder.AddProjectedOrders(
            expectedMonthlyStableCoinsMintedAmount: f.PickRandom<StableCoinsMintedAmount>(),
            expectedMonthlyCumulativeMints: f.PickRandom<CumulativeStableCoinsMints>(),
            maxAnticipatedAmountForSingleMint: f.Random.Double(1_000_000, 100_000_000),
            expectedIrregularMintsFirstYear: f.Random.Number(1, 100)
        );

        application.BusinessInfo?.SourcesOfWealth.Verify(_userInfo);
        application.BusinessInfo?.PoliticallyExposedPerson.Verify(_userInfo);
        application.BusinessInfo?.LinesOfBusiness.Verify(_userInfo);
        application.BusinessInfo?.BusinessActivities.Verify(_userInfo);
        application.BusinessInfo?.ProjectedOrders.Verify(_userInfo);
    }

    private static void SetAuthorizedUsersForm(OnboardingApplication application, Faker f)
    {
        var firstName = f.Person.FirstName;
        var lastName = f.Person.LastName;
        var email = f.Internet.Email(firstName, lastName);

        application.SetAuthorizedUsersForm(firstName, lastName, email,
            GetDocument(f.System.FileName(), "personal-verification"),
            GetDocument(f.System.FileName(), "proof-of-address"),
            _userInfo);
        application.AuthorizedUsers.AuthorizedUsers.ForEach(x => x.Verify(_userInfo));
    }

    private static void SetEntityParticulars(OnboardingApplication application, Faker f)
    {
        var entityParticularDocuments = new List<ApplicationDocument>();
        var epCountry = DataSeedProvider.GetRandomCountry();
        application.SetEntityParticulars(f.PickRandom<CompanyLegalStructure>(), f.Lorem.Sentence(5),
            f.Commerce.Ean13(), DateOnly.FromDateTime(f.Date.PastOffset(5, refDate: _referenceDate).Date), epCountry,
            entityParticularDocuments, f.Address.StreetAddress(), f.Address.ZipCode(), f.Address.State(), epCountry,
            f.Address.City(), f.Person.Email, f.Phone.PhoneNumber(), _userInfo);
        application.EntityParticulars?.Particulars.Verify(_userInfo);
        application.EntityParticulars?.RegistrationAddress.Verify(_userInfo);
        application.EntityParticulars?.Contact.Verify(_userInfo);
    }

    private static List<OnboardingApplication> SeedInReviewApplication(Faker<OnboardingApplication> applicationFaker,
        int count)
    {
        var applicationList = new List<OnboardingApplication>();

        foreach (var _ in Enumerable.Range(0, count))
        {
            var application = applicationFaker.Generate();
            application.Submit(_userInfo);

            applicationList.Add(application);
        }

        return applicationList;
    }

    private static List<OnboardingApplication> SeedApprovedApplication(Faker<OnboardingApplication> applicationFaker,
        int count)
    {
        var applicationList = new List<OnboardingApplication>();

        foreach (var _ in Enumerable.Range(0, count))
        {
            var application = applicationFaker.Generate();
            application.Submit(_userInfo);
            application.Approve(_userInfo);

            applicationList.Add(application);
        }

        return applicationList;
    }

    private static List<OnboardingApplication> SeedRejectedApplication(Faker<OnboardingApplication> applicationFaker,
        int count)
    {
        var applicationList = new List<OnboardingApplication>();

        foreach (var _ in Enumerable.Range(0, count))
        {
            var application = applicationFaker.Generate();
            application.Submit(_userInfo);
            application.Approve(_userInfo);
            application.Reject(_userInfo);

            applicationList.Add(application);
        }

        return applicationList;
    }

    private static List<OnboardingApplication> SeedAdditionalInfoRequiredApplication(
        Faker<OnboardingApplication> applicationFaker, int count)
    {
        var applicationList = new List<OnboardingApplication>();

        foreach (var applicationFormType in Enum.GetValues(typeof(ApplicationFormType)).Cast<ApplicationFormType>())
        {
            foreach (var _ in Enumerable.Range(0, count))
            {
                var application = applicationFaker.Generate();
                application.Submit(_userInfo);
                application.AdditionalInfoRequired(applicationFormType, _userInfo);

                applicationList.Add(application);
            }
        }

        return applicationList;
    }

    private static List<OnboardingApplication> SeedIncompleteApplication(Faker<OnboardingApplication> applicationFaker,
        int count)
    {
        var applicationList = new List<OnboardingApplication>();

        foreach (var _ in Enumerable.Range(0, count))
        {
            var application = applicationFaker.Generate();
            applicationList.Add(application);
        }

        return applicationList;
    }

    public static IReadOnlyCollection<OnboardingApplication> GenerateApplications(int applicationCount)
    {
        var applicationFaker = GetOnboardingApplicationFaker();
        var applicationList = new List<OnboardingApplication>();

        applicationList.AddRange(SeedIncompleteApplication(applicationFaker, applicationCount));
        applicationList.AddRange(SeedInReviewApplication(applicationFaker, applicationCount));
        applicationList.AddRange(SeedAdditionalInfoRequiredApplication(applicationFaker, applicationCount));
        applicationList.AddRange(SeedApprovedApplication(applicationFaker, applicationCount));
        applicationList.AddRange(SeedRejectedApplication(applicationFaker, applicationCount));

        return applicationList;
    }
}
