using Microsoft.Extensions.Options;
using SmartCoinOS.Integrations.CourierApi.Contracts;
using SmartCoinOS.Integrations.CourierApi.DependencyInjection;
using SmartCoinOS.Integrations.CourierApi.Dto;
using SmartCoinOS.Integrations.Shared;

namespace SmartCoinOS.Integrations.CourierApi.Services;

public sealed class CourierApiClient : HttpClientBase, ICourierApiClient
{
    private readonly CourierSettings _settings;

    public CourierApiClient(HttpClient httpClient, IOptions<CourierSettings> courierSettings) : base(httpClient)
    {
        _settings = courierSettings.Value;
    }

    public Task SendCredentialsEmailAsync(string fullName, string email, string pwd,
        CancellationToken cancellationToken)
    {
        var request = new SendCredentialsEmailRequest()
        {
            Message = new SendCredentialMessageRequest
            {
                To = new CredentialMessageRequestTo() { Email = email },
                Template = _settings.FirstTimeLoginTemplate,
                Data = new CredentialMessageRequestData()
                {
                    ClientEmail = email, TemporaryPassword = pwd, FullName = fullName
                }
            }
        };

        return PostAsync("/send", request, cancellationToken);
    }
}
