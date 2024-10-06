using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Results;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.AuthorizedUsers;

internal sealed class SetAuthorizedUsersFormCommandHandler : ICommandHandler<SetAuthorizedUsersFormCommand, ApplicationFormMetadataDto>
{
    private readonly DataContext _context;

    public SetAuthorizedUsersFormCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<ApplicationFormMetadataDto>> Handle(SetAuthorizedUsersFormCommand command, CancellationToken cancellationToken)
    {
        var application = await _context.Applications.FirstAsync(x => x.Id == command.ApplicationId, cancellationToken);

        application.ClearAuthorizedUserForm();

        foreach (var authorizedUser in command.AuthorizedUsers)
        {
            if (authorizedUser.PersonalVerification is null)
                return Result.Fail(new EntityInvalidError(nameof(AuthorizedUserDto), "Missing personal verification document"));

            if (authorizedUser.ProofOfAddress is null)
                return Result.Fail(new EntityInvalidError(nameof(AuthorizedUserDto), "Missing proof of address document"));

            var setResult = application.SetAuthorizedUsersForm(
                authorizedUser.FirstName, authorizedUser.LastName, authorizedUser.UserId,
                authorizedUser.PersonalVerification, authorizedUser.ProofOfAddress, command.UserInfo);

            if (setResult.IsFailed)
                return setResult;
        }

        var response = new ApplicationFormMetadataDto
        {
            PrettyId = application.ApplicationNumber,
            Id = application.Id,
        };

        return Result.Ok(response);
    }
}