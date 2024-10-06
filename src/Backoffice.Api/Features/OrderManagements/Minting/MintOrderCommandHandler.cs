using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Deposit;
using SmartCoinOS.Domain.Orders;
using SmartCoinOS.Integrations.FdtPartnerApi.Contracts;
using SmartCoinOS.Integrations.FdtPartnerApi.Dto;
using SmartCoinOS.Integrations.FdtPartnerApi.Services;
using SmartCoinOS.Integrations.Shared.Exceptions;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Minting;

internal sealed class MintOrderCommandHandler : ICommandHandler<MintOrderCommand, EntityId>
{
    private readonly DataContext _context;
    private readonly OrderService _orderService;
    private readonly IFdtPartnerApiClient _fdtClient;
    private readonly FdtPartnerSettings _fdtSettings;
    private readonly ILogger<MintOrderCommandHandler> _logger;

    public MintOrderCommandHandler(DataContext context, OrderService orderService, IFdtPartnerApiClient fdtClient,
        IOptions<FdtPartnerSettings> fdtOptions, ILogger<MintOrderCommandHandler> logger)
    {
        _context = context;
        _orderService = orderService;
        _fdtClient = fdtClient;
        _fdtSettings = fdtOptions.Value;
        _logger = logger;
    }

    public async Task<Result<EntityId>> Handle(MintOrderCommand command, CancellationToken cancellationToken)
    {
        var mintRequest = command.Request;
        var clientId = mintRequest.ClientId;

        var depositBank = await GetDefaultDepositBankAsync(clientId);
        var clientStatus = await _context.Clients.Where(x => x.Id == clientId).Select(x => x.Status)
            .FirstAsync(cancellationToken);
        var depositWallet = await GetDefaultOrFirstDepositWalletAsync(cancellationToken);
        var fdtDepositAccount = await _context.FdtDepositAccounts.FirstAsync(cancellationToken);

        var clientOrderData = new ClientOrderData()
        {
            ClientId = mintRequest.ClientId,
            WalletId = mintRequest.WalletId,
            BankAccountId = mintRequest.BankAccountId,
            FdtAccountId = mintRequest.FdtAccountId,
            ClientStatus = clientStatus
        };

        var fdtOrderData = new FdtOrderData()
        {
            FdtAccountId = fdtDepositAccount.Id,
            WalletId = depositWallet,
            BankId = depositBank
        };


        var order = await _orderService.CreateMintOrderAsync(clientOrderData, fdtOrderData, mintRequest.MintAmount,
            command.UserInfo, cancellationToken);

        var depositResult = await SetupBankDepositInstructionsAsync(order, cancellationToken);
        if (depositResult.IsFailed)
            return depositResult;

        _context.MintOrders.Add(order);

        return Result.Ok(new EntityId(order.Id.Value));
    }

    private async Task<Result> SetupBankDepositInstructionsAsync(MintOrder order, CancellationToken cancellationToken)
    {
        if (order.BankAccountId.HasValue)
        {
            var smartTrustBankId =
                await GetSmartTrustBankIdAsync(order.ClientId, order.BankAccountId, cancellationToken);
            var depositInstructionsResult =
                await CreateFiatDepositInstructionAsync(order, smartTrustBankId, cancellationToken);

            if (depositInstructionsResult.IsFailed)
                return Result.Fail(depositInstructionsResult.Errors);

            var depositInstructions = depositInstructionsResult.Value;
            return order.CreateDepositInstruction(depositInstructions.Id, depositInstructions.ReferenceNumber);
        }

        return Result.Ok();
    }

    private async Task<DepositBankId> GetDefaultDepositBankAsync(ClientId clientId)
    {
        var clientDefaultDeposit = await _context.Clients.Where(x => x.Id == clientId)
                                       .Select(x => x.DepositBankId).FirstOrDefaultAsync()
                                   ?? await _context.DepositBanks.Where(x => x.Default)
                                       .Select(x => x.Id).FirstAsync();

        return clientDefaultDeposit;
    }

    private async Task<DepositWalletId> GetDefaultOrFirstDepositWalletAsync(CancellationToken cancellationToken)
    {
        var depositWallets = await _context.DepositWallets.ToListAsync(cancellationToken);

        var defaultDeposit = depositWallets.Find(x => x.Default);

        if (defaultDeposit is not null)
            return defaultDeposit.Id;

        return depositWallets.Select(x => x.Id).First();
    }

    private async Task<int> GetSmartTrustBankIdAsync(ClientId clientId, BankAccountId? bankAccountId,
        CancellationToken cancellationToken)
    {
        var client = await _context.Clients.FirstAsync(x => x.Id == clientId, cancellationToken);
        var bankAccount = client.BankAccounts.First(x => x.Id == bankAccountId);
        return bankAccount.GetIncomingSmartTrustBank();
    }

    private async Task<Result<CreateFiatDepositResponse>> CreateFiatDepositInstructionAsync(MintOrder order,
        int smartTrustBankId, CancellationToken cancellationToken)
    {
        try
        {
            return await _fdtClient.CreateFiatDepositInstructionAsync(
                new CreateFiatDepositRequest
                {
                    ClientId = _fdtSettings.FdtClientId,
                    TargetServiceAccountId = _fdtSettings.ServiceAccountId,
                    SourceBankAccountId = smartTrustBankId,
                    Amount = order.OrderedAmount.Amount,
                    SourceOfFunds = SourceOfFunds.Others
                }, cancellationToken);
        }
        catch (ApiException ae)
        {
            _logger.LogError(ae, "Failed to contact FDT Partner API with error:{message}", ae.Message);
            return Result.Fail("FDT API failed. Can't create order.");
        }
    }
}
