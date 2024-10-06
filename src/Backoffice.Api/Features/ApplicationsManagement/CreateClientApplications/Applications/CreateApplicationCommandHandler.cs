using FluentResults;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Application;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.Applications;

internal sealed class CreateApplicationCommandHandler : ICommandHandler<CreateApplicationCommand, ApplicationFormMetadataDto>
{
    private readonly DataContext _context;
    private readonly ApplicationService _applicationService;

    public CreateApplicationCommandHandler(DataContext context, ApplicationService applicationService)
    {
        _context = context;
        _applicationService = applicationService;
    }

    public async Task<Result<ApplicationFormMetadataDto>> Handle(CreateApplicationCommand request, CancellationToken cancellationToken)
    {
        var application = await _applicationService.CreateAsync(request.Application.LegalEntityName, request.UserInfo, cancellationToken);
        _context.Applications.Add(application);

        var response = new ApplicationFormMetadataDto
        {
            Id = application.Id,
            PrettyId = application.ApplicationNumber
        };

        return Result.Ok(response);
    }
}