using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.ReviewClientApplications.Applications;

internal sealed class RejectApplicationCommandHandler : ICommandHandler<RejectApplicationCommand, ApplicationFormMetadataDto>
{
    private readonly DataContext _context;

    public RejectApplicationCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<ApplicationFormMetadataDto>> Handle(RejectApplicationCommand request, CancellationToken cancellationToken)
    {
        var application = await _context.Applications.FirstAsync(x => x.Id == request.ApplicationId, cancellationToken);

        var rejectResult = application.Reject(request.UserInfo);

        if (rejectResult.IsFailed)
            return rejectResult;

        var response = new ApplicationFormMetadataDto
        {
            PrettyId = application.ApplicationNumber,
            Id = application.Id,
        };

        return Result.Ok(response);
    }
}