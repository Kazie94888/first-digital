using SmartCoinOS.Integrations.GnosisSafeApi.Contracts;
using SmartCoinOS.Integrations.GnosisSafeApi.Dto;
using SmartCoinOS.Integrations.Shared;

namespace SmartCoinOS.Integrations.GnosisSafeApi.Services;

public sealed class GnosisSafeApiClient : HttpClientBase, IGnosisSafeApiClient
{
    public GnosisSafeApiClient(HttpClient httpClient) : base(httpClient) { }

    public Task<FindMySafesResponse> FindOwnerSafes(GnosisSafeNetwork network, string ownerAddress,
        CancellationToken cancellationToken)
    {
        var url = GetUrl(network, $"/v1/owners/{ownerAddress}/safes/");
        return GetAsync<FindMySafesResponse>(url, cancellationToken);
    }

    public Task<SafeResponse> GetSafe(GnosisSafeNetwork network, string safeAddress,
        CancellationToken cancellationToken)
    {
        var url = GetUrl(network, $"/v1/safes/{safeAddress}/");
        return GetAsync<SafeResponse>(url, cancellationToken);
    }

    public Task<MultisigTransactionResponse> GetMultisigTransaction(GnosisSafeNetwork network, string safeTxHash,
        CancellationToken cancellationToken)
    {
        var url = GetUrl(network, $"/v1/multisig-transactions/{safeTxHash}/");
        return GetAsync<MultisigTransactionResponse>(url, cancellationToken);
    }

    public Task<ConfirmationsListResponse> GetMultisignTransactionConfirmationsAsync(GnosisSafeNetwork network,
        string safeTxHash, CancellationToken cancellationToken)
    {
        var url = GetUrl(network, $"/v1/multisig-transactions/{safeTxHash}/confirmations/");
        return GetAsync<ConfirmationsListResponse>(url, cancellationToken);
    }

    public Task CreateMultisigTransactionAsync(GnosisSafeNetwork network, string address,
        CreateMultisigTransactionRequest request, CancellationToken cancellationToken)
    {
        var url = GetUrl(network, $"/v1/safes/{address}/multisig-transactions/");
        return PostAsync(url, request, cancellationToken);
    }

    public Task<EstimateSafeTxGasForTransactionResponse> EstimateSafeTxGasAsync(GnosisSafeNetwork network,
        string address, EstimateSafeTxGasForTransactionRequest request, CancellationToken cancellationToken)
    {
        var url = GetUrl(network, $"/v1/safes/{address}/multisig-transactions/estimations");
        return PostAsync<EstimateSafeTxGasForTransactionRequest, EstimateSafeTxGasForTransactionResponse>(url, request,
            cancellationToken);
    }


    private static string GetUrl(GnosisSafeNetwork network, string url)
    {
        var host = network switch
        {
            GnosisSafeNetwork.Etherium => "https://safe-transaction-mainnet.safe.global",
            GnosisSafeNetwork.Sepolia => "https://safe-transaction-sepolia.safe.global",
            GnosisSafeNetwork.Bnb => "https://safe-transaction-bsc.safe.global",
            _ => throw new ArgumentOutOfRangeException(network.ToString())
        };

        return $"{host}/api{url}";
    }
}