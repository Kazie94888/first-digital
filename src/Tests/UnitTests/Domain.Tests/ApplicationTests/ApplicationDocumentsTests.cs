using SmartCoinOS.Domain.Application;

namespace SmartCoinOS.Tests.Domain.ApplicationTests;

public sealed class ApplicationDocumentsTests : BaseApplicationTest
{
    [Fact]
    public void AddDocuments_WithValidData_Successfully()
    {
        var application = GetApplicationFromFactory();
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

        var result = application.SetDocuments(sampleDocumentRecord);

        Assert.True(result.IsSuccess);
    }
}
