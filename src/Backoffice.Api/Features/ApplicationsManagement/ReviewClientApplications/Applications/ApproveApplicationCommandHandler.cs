using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.ReviewClientApplications.Applications;

internal sealed class ApproveApplicationCommandHandler : ICommandHandler<ApproveApplicationCommand, ApplicationFormMetadataDto>
{
    private readonly DataContext _context;

    public ApproveApplicationCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<ApplicationFormMetadataDto>> Handle(ApproveApplicationCommand request, CancellationToken cancellationToken)
    {
        var application = await _context.Applications.FirstAsync(x => x.Id == request.ApplicationId, cancellationToken);

        var approveResult = application.Approve(request.UserInfo);

        if (approveResult.IsFailed)
            return approveResult;

        var response = new ApplicationFormMetadataDto
        {
            PrettyId = application.ApplicationNumber,
            Id = application.Id,
        };

        return Result.Ok(response);
    }
}