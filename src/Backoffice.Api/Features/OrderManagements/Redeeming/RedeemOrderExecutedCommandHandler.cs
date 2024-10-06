using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Kiota.Abstractions;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Backoffice.Api.Base.Options;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Orders;
using SmartCoinOS.Integrations.FdtPartnerApi.Contracts;
using SmartCoinOS.Integrations.FdtPartnerApi.Dto;
using SmartCoinOS.Integrations.FdtPartnerApi.Services;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Redeeming;

internal sealed class RedeemOrderExecutedCommandHandler : ICommandHandler<RedeemOrderExecutedCommand, EntityId>
{
    readonly DataContext _context;
    readonly BlockchainSettings _settings;
    readonly IFdtPartnerApiClient _fdtClient;
    readonly FdtPartnerSettings _fdtSettings;
    readonly ILogger<RedeemOrderExecutedCommandHandler> _logger;

    public RedeemOrderExecutedCommandHandler(DataContext context, IOptionsSnapshot<BlockchainSettings> settings,
        IFdtPartnerApiClient fdtClient, IOptions<FdtPartnerSettings> fdtSettings, ILogger<RedeemOrderExecutedCommandHandler> logger)
    {
        _context = context;
        _settings = settings.Value;
        _fdtClient = fdtClient;
        _fdtSettings = fdtSettings.Value;
        _logger = logger;
    }

    public async Task<Result<EntityId>> Handle(RedeemOrderExecutedCommand command, CancellationToken cancellationToken)
    {
        var order = await _context.RedeemOrders.FirstAsync(x => x.Id == command.OrderId, cancellationToken);
        var request = command.Request;

        var alias = _settings.Signers.GetValueOrDefault(request.Signature.Address);
        var safeSignature = new SafeSignature(request.Signature.Address, alias, request.Signature.SubmissionDate);

        var executionResult = order.Executed(safeSignature, command.UserInfo);
        if (executionResult.IsFailed)
            return executionResult;

        if (order.IsBankTransfer())
        {
            var fdtInstruction = await CreatePaymentInstructionAsync(order, cancellationToken);

            if (fdtInstruction.IsFailed)
                return Result.Fail(fdtInstruction.Errors);

            var orderInstrResult = order.PaymentInstructionCreated(fdtInstruction.Value.Id, fdtInstruction.Value.ReferenceNumber);
            if (orderInstrResult.IsFailed)
                return orderInstrResult;
        }

        return Result.Ok(new EntityId(order.Id.Value));
    }

    private async Task<int> GetSmartTrustBankIdAsync(ClientId clientId, BankAccountId? bankAccountId, CancellationToken cancellationToken)
    {
        var client = await _context.Clients.FirstAsync(x => x.Id == clientId, cancellationToken);
        var bankAccount = client.BankAccounts.First(x => x.Id == bankAccountId);
        return bankAccount.GetOutgoingSmartTrustBank();
    }

    private async Task<Result<CreateFiatPaymentInstructionResponse>> CreatePaymentInstructionAsync(Order order, CancellationToken cancellationToken)
    {
        var smartTrustBankId = await GetSmartTrustBankIdAsync(order.ClientId, order.BankAccountId, cancellationToken);

        try
        {
            var fdtInstruction = await _fdtClient.CreateFiatPaymentInstructionAsync(new CreateFiatPaymentInstructionRequest
            {
                ClientId = _fdtSettings.FdtClientId,
                SourceServiceAccountId = _fdtSettings.ServiceAccountId,
                PaymentReference = order.OrderNumber.Value,
                PurposeOfPayment = PurposeOfPayment.Investment,
                TargetBankAccountId = smartTrustBankId,
                Amount = order.ActualAmount!.Amount
            }, cancellationToken);

            return Result.Ok(fdtInstruction);
        }
        catch (ApiException ae)
        {
            _logger.LogError(ae, "Failed to contact FDT Partner API with error:{message}", ae.Message);
            return Result.Fail("FDT API failed. Can't create payment instruction.");
        }
    }
}