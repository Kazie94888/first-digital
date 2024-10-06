namespace SmartCoinOS.Integrations.CourierApi.Contracts;
public interface ICourierApiClient
{
    Task SendCredentialsEmailAsync(string fullName, string email, string pwd, CancellationToken cancellationToken);
}
