namespace SmartCoinOS.Domain.Application;

public interface IApplicationNumberService
{
    Task<ApplicationNumber> GetApplicationNumberAsync(CancellationToken cancellationToken);
}
