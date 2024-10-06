using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.ReviewClientApplications.Sections;

internal sealed class VerifySectionCommandHandler : ICommandHandler<VerifySectionCommand, ApplicationFormMetadataDto>
{
    private readonly DataContext _context;

    public VerifySectionCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<ApplicationFormMetadataDto>> Handle(VerifySectionCommand command, CancellationToken cancellationToken)
    {
        var application = await _context.Applications.FirstAsync(x => x.Id == command.ApplicationId, cancellationToken);

        var recordOrId = command.Request.Record ?? command.Request.Id;

        if (string.IsNullOrEmpty(recordOrId))
            return Result.Fail("Verifiable record or verifiable identifier was not found.");

        var verificationResult = application.VerifyFormRecord(command.Request.Form, recordOrId, command.UserInfo);

        if (verificationResult.IsFailed)
            return verificationResult;

        var result = new ApplicationFormMetadataDto
        {
            Id = application.Id,
            PrettyId = application.ApplicationNumber.Value.ToString(),
        };

        return Result.Ok(result);
    }
}