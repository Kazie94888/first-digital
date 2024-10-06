using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.Applications;

internal sealed class SubmitApplicationCommandHandler : ICommandHandler<SubmitApplicationCommand, ApplicationFormMetadataDto>
{
    private readonly DataContext _context;

    public SubmitApplicationCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<ApplicationFormMetadataDto>> Handle(SubmitApplicationCommand request, CancellationToken cancellationToken)
    {
        var application = await _context.Applications.FirstAsync(x => x.Id == request.ApplicationId, cancellationToken);

        var submitResult = application.Submit(request.UserInfo);

        if (submitResult.IsFailed)
            return submitResult;

        var response = new ApplicationFormMetadataDto
        {
            PrettyId = application.ApplicationNumber,
            Id = application.Id,
        };

        return Result.Ok(response);
    }
}