using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Application.Enums;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.EntityParticulars;

internal sealed class GetEntityParticularsFormQueryHandler : IQueryHandler<GetEntityParticularsFormQuery, ApplicationFormDto>
{
    private readonly ReadOnlyDataContext _context;

    public GetEntityParticularsFormQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<ApplicationFormDto>> Handle(GetEntityParticularsFormQuery request, CancellationToken cancellationToken)
    {
        var application = await _context.Applications.FirstAsync(x => x.Id == request.ApplicationId, cancellationToken);

        var epForm = application.EntityParticulars;
        var particularsForm = new VerifiableSectionDto<CompanyParticular>();

        if (epForm?.Particulars is not null)
        {
            var section = epForm.Particulars;

            particularsForm.Set(section.IsVerified(), new CompanyParticular
            {
                LegalStructure = section.LegalStructure,
                StructureDetails = section.StructureDetails,
                CountryOfInc = section.CountryOfInc,
                DateOfInc = section.DateOfInc,
                RegistrationNumber = section.RegistrationNumber,
                Documents = section.Documents?.Select(x => (DocumentDto)x).ToList() ?? []
            });
        }

        var addressForm = new VerifiableSectionDto<RegistrationAddress>();
        if (epForm?.RegistrationAddress is not null)
        {
            var section = epForm.RegistrationAddress;
            addressForm.Set(section.IsVerified(), new RegistrationAddress
            {
                Street = section.Street,
                Country = section.Country,
                PostalCode = section.PostalCode,
                State = section.State,
                City = section.City
            });
        }

        var contactForm = new VerifiableSectionDto<CompanyContact>();
        if (epForm?.Contact is not null)
        {
            var section = epForm.Contact;

            contactForm.Set(section.IsVerified(), new CompanyContact
            {
                Email = section.Email,
                Phone = section.Phone
            });
        }

        var applicationForm = new ApplicationFormDto(ApplicationFormType.EntityParticulars);

        applicationForm.AddSection(ApplicationFormNames.EntityParticularRecords.CompanyParticulars, particularsForm);
        applicationForm.AddSection(ApplicationFormNames.EntityParticularRecords.RegistrationAddress, addressForm);
        applicationForm.AddSection(ApplicationFormNames.EntityParticularRecords.ContactForm, contactForm);

        return Result.Ok(applicationForm);
    }
}