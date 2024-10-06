using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.ReviewClientApplications.Applications;

internal sealed class ArchiveApplicationCommandHandler : ICommandHandler<ArchiveApplicationCommand, ApplicationFormMetadataDto>
{
    private readonly DataContext _context;

    public ArchiveApplicationCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<ApplicationFormMetadataDto>> Handle(ArchiveApplicationCommand request, CancellationToken cancellationToken)
    {
        var application = await _context.Applications.FirstAsync(x => x.Id == request.ApplicationId, cancellationToken);

        var archiveResult = application.Archive(request.UserInfo);

        if (archiveResult.IsFailed)
            return archiveResult;

        var response = new ApplicationFormMetadataDto
        {
            PrettyId = application.ApplicationNumber,
            Id = application.Id,
        };

        return Result.Ok(response);
    }
}