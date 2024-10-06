using SmartCoinOS.Domain.Application;
using SmartCoinOS.Domain.Deposit;
using SmartCoinOS.Domain.Exceptions;
using SmartCoinOS.Domain.SeedWork;
using SmartCoinOS.Domain.Shared;

namespace SmartCoinOS.Domain.Clients;

public sealed class ClientBuilder
{
    private EntityParticular? _entityParticular;
    private Address? _registrationAddress;
    private CompanyContact? _companyContact;

    private string? _applicationNumber;
    private DepositBankId? _defaultDeposit;

    private readonly List<ClientDocument> _documents = [];

    private readonly UserInfo _userInfo;

    public ClientBuilder(UserInfo userInfo)
    {
        _userInfo = userInfo;
    }

    public ClientBuilder AddEntityParticulars(string legalEntityName,
        string registrationNumber,
        DateOnly dateOfInc,
        CompanyLegalStructure legalStructure,
        string? otherLegalStructure,
        string countryOfInc)
    {
        _entityParticular = new EntityParticular
        {
            Id = EntityParticularId.New(),
            LegalEntityName = legalEntityName,
            LegalStructure = legalStructure,
            StructureDetails = otherLegalStructure,
            CountryOfInc = countryOfInc,
            RegistrationNumber = registrationNumber,
            DateOfIncorporation = dateOfInc,
            CreatedBy = _userInfo
        };

        return this;
    }

    public ClientBuilder AddAddress(string country,
        string? state,
        string postalCode,
        string city,
        string street)
    {
        _registrationAddress = new Address
        {
            Id = AddressId.New(),
            Country = country,
            City = city,
            PostalCode = postalCode,
            Street = street,
            State = state,
            CreatedBy = _userInfo
        };

        return this;
    }

    public ClientBuilder AddContact(string email, string phone)
    {
        _companyContact = new CompanyContact
        {
            Email = email,
            Phone = phone
        };
        return this;
    }

    public ClientBuilder AddMetadata(string applicationNumber, DepositBankId? defaultDeposit)
    {
        _applicationNumber = applicationNumber;
        _defaultDeposit = defaultDeposit;

        return this;
    }

    public ClientBuilder AddDocument(DocumentId documentId, string? fileName, string docType)
    {
        _documents.Add(new ClientDocument()
        {
            DocumentId = documentId,
            DocumentType = docType,
            FileName = fileName
        });

        return this;
    }

    public Client Build()
    {
        if (_entityParticular is null || _registrationAddress is null
                                      || _companyContact is null || string.IsNullOrEmpty(_applicationNumber))
            throw new EntityInvalidStateException(
                "Cannot build client because one or more required attributes are missing.");

        var client = Client.Create(_entityParticular, _registrationAddress, _companyContact, _applicationNumber,
            _defaultDeposit, _userInfo);

        foreach (var doc in _documents)
        {
            var docResult = client.AddDocument(doc.DocumentId, doc.FileName, doc.DocumentType, _userInfo);
            if (docResult.IsFailed)
                throw new EntityInvalidStateException(
                    $"Cannot build client, failed to apped documents: {docResult.Errors.FirstOrDefault()}.");
        }

        return client;
    }
}
