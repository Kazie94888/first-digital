using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Domain.Application;
using SmartCoinOS.Domain.Exceptions;
using SmartCoinOS.Persistence;

namespace SmartCoinOS.Infrastructure.Services;

internal class ApplicationNumberService : IApplicationNumberService
{
    private readonly DataContext _dataContext;

    public ApplicationNumberService(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    private const int _maxAttempts = 10;

    public async Task<ApplicationNumber> GetApplicationNumberAsync(CancellationToken cancellationToken)
    {
        var attempt = 0;
        var applicationNumber = ApplicationNumber.New();
        var existsAlready =
            await _dataContext.Applications.AnyAsync(o => o.ApplicationNumber == applicationNumber, cancellationToken);

        while (existsAlready)
        {
            attempt++;
            if (attempt > _maxAttempts)
                throw new RetriesExceededException(
                    "Attempts exceeded while trying to find a unique application number.");

            applicationNumber = ApplicationNumber.New();
            existsAlready =
                await _dataContext.Applications.AnyAsync(o => o.ApplicationNumber == applicationNumber,
                    cancellationToken);
        }

        return applicationNumber;
    }
}
