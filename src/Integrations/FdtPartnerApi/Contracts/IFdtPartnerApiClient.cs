using SmartCoinOS.Integrations.FdtPartnerApi.Dto;

namespace SmartCoinOS.Integrations.FdtPartnerApi.Contracts;

public interface IFdtPartnerApiClient
{
    Task<CreateFiatDepositResponse> CreateFiatDepositInstructionAsync(CreateFiatDepositRequest request,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<FiatDepositInstructionTransaction>> GetDepositTransactionsAsync(int depositInstructionId,
        CancellationToken cancellationToken);

    Task<CreateFiatPaymentInstructionResponse> CreateFiatPaymentInstructionAsync(
        CreateFiatPaymentInstructionRequest request, CancellationToken cancellationToken);
}
