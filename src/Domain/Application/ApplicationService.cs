using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Application;

public sealed class ApplicationService
{
    private readonly IApplicationNumberService _applicationNumberService;

    public ApplicationService(IApplicationNumberService applicationNumberService)
    {
        _applicationNumberService = applicationNumberService;
    }

    public async Task<Application> CreateAsync(
        string legalEntityName,
        UserInfo createdBy,
        CancellationToken cancellationToken)
    {
        var applicationNumber = await _applicationNumberService.GetApplicationNumberAsync(cancellationToken);

        ArgumentNullException.ThrowIfNull(applicationNumber);

        var application = Application.Create(legalEntityName, applicationNumber, createdBy);
        return application;
    }
}
