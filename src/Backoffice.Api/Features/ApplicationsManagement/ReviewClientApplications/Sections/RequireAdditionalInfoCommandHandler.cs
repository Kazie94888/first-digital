using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.ReviewClientApplications.Sections;

internal sealed class RequireAdditionalInfoCommandHandler : ICommandHandler<RequireAdditionalInfoCommand, ApplicationFormMetadataDto>
{
    private readonly DataContext _context;

    public RequireAdditionalInfoCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<ApplicationFormMetadataDto>> Handle(RequireAdditionalInfoCommand command, CancellationToken cancellationToken)
    {
        var application = await _context.Applications.FirstAsync(x => x.Id == command.ApplicationId, cancellationToken);

        var aditionalInfoRequiredResult = application.AdditionalInfoRequired(command.Body.Form, command.UserInfo);

        if (aditionalInfoRequiredResult.IsFailed)
            return aditionalInfoRequiredResult;

        var response = new ApplicationFormMetadataDto
        {
            PrettyId = application.ApplicationNumber,
            Id = application.Id,
        };

        return Result.Ok(response);
    }
}