using SmartCoinOS.Integrations.FdtPartnerApi.Contracts;
using SmartCoinOS.Integrations.FdtPartnerApi.Dto;
using SmartCoinOS.Integrations.Shared;

namespace SmartCoinOS.Integrations.FdtPartnerApi.Services;

public sealed class FdtPartnerApiClient : HttpClientBase, IFdtPartnerApiClient
{
    public FdtPartnerApiClient(HttpClient httpClient) : base(httpClient) { }

    public Task<CreateFiatDepositResponse> CreateFiatDepositInstructionAsync(CreateFiatDepositRequest request,
        CancellationToken cancellationToken)
    {
        var url = "/instructions/deposits/fiat";
        return HttpPostAsync<CreateFiatDepositRequest, CreateFiatDepositResponse>(url, request, cancellationToken);
    }

    public Task<IReadOnlyList<FiatDepositInstructionTransaction>> GetDepositTransactionsAsync(int depositInstructionId,
        CancellationToken cancellationToken)
    {
        var url = $"/instructions/deposits/fiat/{depositInstructionId}/transactions";
        return HttpGetAsync<IReadOnlyList<FiatDepositInstructionTransaction>>(url, cancellationToken);
    }

    public Task<CreateFiatPaymentInstructionResponse> CreateFiatPaymentInstructionAsync(
        CreateFiatPaymentInstructionRequest request, CancellationToken cancellationToken)
    {
        var url = "/instructions/payments/fiat";
        return HttpPostAsync<CreateFiatPaymentInstructionRequest, CreateFiatPaymentInstructionResponse>(url, request,
            cancellationToken);
    }

    private async Task<TResponse> HttpPostAsync<TRequest, TResponse>(string url, TRequest request,
        CancellationToken cancellationToken)
    {
        return await PostAsync<TRequest, TResponse>(url, request, cancellationToken);
    }

    private async Task<TResponse> HttpGetAsync<TResponse>(string url, CancellationToken cancellationToken)
    {
        return await GetAsync<TResponse>(url, cancellationToken);
    }
}
