using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Application;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.EntityParticulars;

internal sealed class SetEntityParticularsFormCommandHandler : ICommandHandler<SetEntityParticularsFormCommand, ApplicationFormMetadataDto>
{
    private readonly DataContext _context;

    public SetEntityParticularsFormCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<ApplicationFormMetadataDto>> Handle(SetEntityParticularsFormCommand command, CancellationToken cancellationToken)
    {
        var application = await _context.Applications.FirstAsync(x => x.Id == command.ApplicationId, cancellationToken);

        var company = command.EntityParticularsForm.CompanyParticulars;
        var address = command.EntityParticularsForm.RegistrationAddress;
        var contact = command.EntityParticularsForm.CompanyContact;

        var documents = company.Documents.Select(x => (ApplicationDocument)x).ToList();

        var epResult = application.SetEntityParticulars(company.LegalStructure, company.StructureDetails, company.RegistrationNumber,
                company.DateOfInc, company.CountryOfInc, documents, address.Street, address.PostalCode, address.State, address.Country, address.City,
                contact.Email, contact.Phone, command.UserInfo);

        if (epResult.IsFailed)
            return epResult;

        var response = new ApplicationFormMetadataDto
        {
            PrettyId = application.ApplicationNumber,
            Id = application.Id,
        };

        return Result.Ok(response);
    }
}