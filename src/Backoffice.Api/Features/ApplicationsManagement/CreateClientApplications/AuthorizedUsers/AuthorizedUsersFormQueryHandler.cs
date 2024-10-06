using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Application.Enums;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.AuthorizedUsers;

internal sealed class AuthorizedUsersFormQueryHandler : IQueryHandler<AuthorizedUsersFormQuery, ApplicationFormDto>
{
    private readonly ReadOnlyDataContext _context;

    public AuthorizedUsersFormQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<ApplicationFormDto>> Handle(
        AuthorizedUsersFormQuery request,
        CancellationToken cancellationToken)
    {
        var application =
            await _context.Applications.FirstAsync(app => app.Id == request.ApplicationId, cancellationToken);

        var form = application.AuthorizedUsers;
        var applicationForm = new ApplicationFormDto(ApplicationFormType.AuthorizedUsers);
        foreach (var record in form.AuthorizedUsers)
        {
            applicationForm.AddItem(new VerifiableSectionDto<AuthorizedUserDto>
            {
                Id = record.Id.Value,
                Verified = record.IsVerified(),
                Data = new AuthorizedUserDto
                {
                    FirstName = record.FirstName,
                    LastName = record.LastName,
                    UserId = record.Email,
                    PersonalVerification = record.PersonalVerificationDocument is not null ?
                                                (DocumentDto)record.PersonalVerificationDocument : null,
                    ProofOfAddress = record.ProofOfAddressDocument is not null ?
                                                (DocumentDto)record.ProofOfAddressDocument : null
                }
            });
        }

        return Result.Ok(applicationForm);
    }
}