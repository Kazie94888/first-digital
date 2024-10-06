using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.AuthorizedUsers;

public sealed record SetAuthorizedUsersFormCommand : ICommand<ApplicationFormMetadataDto>
{
    [FromRoute(Name = "applicationId")]
    public required ApplicationId ApplicationId { get; init; }

    [FromBody]
    public required List<AuthorizedUserDto> AuthorizedUsers { get; init; }

    public required UserInfoDto UserInfo { get; init; }
}

public sealed record AuthorizedUserDto
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string UserId { get; init; }
    public DocumentDto? PersonalVerification { get; init; }
    public DocumentDto? ProofOfAddress { get; init; }
}