using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.ClientPortal.Api.Base.Dto;
using SmartCoinOS.Domain.Orders;
using SmartCoinOS.Persistence;

namespace SmartCoinOS.ClientPortal.Api.Features.OrderInsights.Details;

internal sealed class MintOrderDetailsQueryHandler : IQueryHandler<MintOrderDetailsQuery, MintOrderDetailsResponse>
{
    private readonly ReadOnlyDataContext _context;

    public MintOrderDetailsQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<MintOrderDetailsResponse>> Handle(MintOrderDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var clientId = request.UserInfo.ClientId;

        var order = await _context.Orders.FirstAsync(o => o.Id == request.OrderId && o.ClientId == clientId,
            cancellationToken: cancellationToken);

        var response = new MintOrderDetailsResponse
        {
            Overview = GetProgressOverview(order),
            Summary = await GetOrderSummaryAsync(request, cancellationToken),
            MintingDetails = await GetMintingDetailsAsync(request.OrderId, cancellationToken)
        };

        if (order.IsBankTransfer())
            response.BankDetails = await GetBankDetailsAsync(request.OrderId, cancellationToken);
        else
            response.RsnDetails = await GetRsnDetailsAsync(request.OrderId, cancellationToken);

        return Result.Ok(response);
    }

    private async Task<MintOrderSummary> GetOrderSummaryAsync(MintOrderDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var queryResult = await (
            from order in _context.Orders
            join client in _context.Clients on order.ClientId equals client.Id
            join entityParticular in _context.EntityParticulars on client.Id equals entityParticular.ClientId
            join orderWallet in _context.Wallets on order.WalletId equals orderWallet.Id
            where order.Id == request.OrderId && !entityParticular.Archived
            select new MintOrderSummary
            {
                OrderNumber = order.OrderNumber.Value,
                OrderStatus = order.Status,
                OrderProcessingStatus = order.ProcessingStatus,
                ClientId = order.ClientId,
                ClientName = entityParticular.LegalEntityName,
                ClientStatus = client.Status,
                OrderedAmount =
                    new MoneyDto { Amount = order.OrderedAmount.Amount, AssetSymbol = order.OrderedAmount.Currency },
                Network = orderWallet.WalletAccount.Network
            }).FirstAsync(cancellationToken: cancellationToken);

        return queryResult;
    }

    private async Task<MintBankDetails> GetBankDetailsAsync(OrderId orderId, CancellationToken cancellationToken)
    {
        var queryResult = await (
            from order in _context.Orders
            join orderBank in _context.BankAccounts on order.BankAccountId equals orderBank.Id
            join orderDepositBank in _context.DepositBanks on order.DepositBankId equals orderDepositBank.Id
            join address in _context.Addresses on orderDepositBank.Address.Id equals address.Id
            where order.Id == orderId
            select new
            {
                FromBankAccountName = orderBank.BankName,
                FromIban = orderBank.Iban,
                ToBankAccountName = orderDepositBank.Name,
                ToIban = orderDepositBank.Iban,
                ToBankAddress = address,
                ToSwift = orderDepositBank.Swift,
                ReferenceNumber = order.DepositInstruction != null ? order.DepositInstruction.ReferenceNumber : ""
            }).FirstAsync(cancellationToken);

        return new MintBankDetails
        {
            FromBankAccountName = queryResult.FromBankAccountName,
            FromIban = queryResult.FromIban,
            ToBankAccountName = queryResult.ToBankAccountName,
            ToIban = queryResult.ToIban,
            ToBankBranchAddress = queryResult.ToBankAddress.GetFullAddress(),
            ToSwift = queryResult.ToSwift,
            ReferenceNumber = queryResult.ReferenceNumber
        };
    }

    private async Task<MintRsnDetails> GetRsnDetailsAsync(OrderId orderId, CancellationToken cancellationToken)
    {
        var depositResult = await (from order in _context.Orders
            join clientFdtAccount in _context.FdtAccounts on order.FdtAccountId equals clientFdtAccount.Id
            join toDepositFdt in _context.FdtDepositAccounts on order.DepositFdtAccountId equals toDepositFdt.Id into
                depositGroup
            from depositFdtAccount in depositGroup.DefaultIfEmpty()
            where order.Id == orderId
            select new { clientFdtAccount, order.RsnReference, depositFdtAccount }).FirstAsync(cancellationToken);

        var depositDetails = new MintRsnDetails
        {
            FromFdtAccount =
                new FdtAccountDto
                {
                    Name = depositResult.clientFdtAccount.ClientName,
                    Number = depositResult.clientFdtAccount.AccountNumber
                },
            TransactionReferenceNumber = depositResult.RsnReference?.TransactionId,
            ToFdtAccount = depositResult.depositFdtAccount is null
                ? null
                : new FdtAccountDto
                {
                    Name = depositResult.depositFdtAccount.AccountName,
                    Number = depositResult.depositFdtAccount.AccountNumber
                }
        };

        return depositDetails;
    }

    private async Task<MintingDetails> GetMintingDetailsAsync(OrderId orderId, CancellationToken cancellationToken)
    {
        var mintingResult = await (from order in _context.Orders
            join orderWallet in _context.Wallets on order.WalletId equals orderWallet.Id
            join depositWallet in _context.DepositWallets on order.DepositWalletId equals depositWallet.Id into
                depositWalletGroup
            from depositWallet in depositWalletGroup.DefaultIfEmpty()
            where order.Id == orderId
            select new
            {
                order.OrderedAmount,
                order.DepositedAmount,
                order.ProcessingStatus,
                MintingAmount = order.ActualAmount,
                TransactionHash = order.SafeTxHash,
                orderWallet.WalletAccount.Network,
                DepositWalletAddress = depositWallet != null ? depositWallet.Account.Address : null,
                ToWalletAddress = orderWallet.WalletAccount.Address,
                ToWalletUsageCount = orderWallet.OrdersCount
            }).FirstAsync(cancellationToken);

        var mintingDetails = new MintingDetails
        {
            DepositAmount =
                mintingResult.DepositedAmount == null
                    ? null
                    : new MoneyDto
                    {
                        Amount = mintingResult.DepositedAmount.Amount,
                        AssetSymbol = mintingResult.DepositedAmount.Currency
                    },
            TransactionHash = mintingResult.TransactionHash,
            Network = mintingResult.Network,
            FromWalletAddress = mintingResult.DepositWalletAddress,
            ToWalletAddress = mintingResult.ToWalletAddress,
            ToWalletUsageCount = mintingResult.ToWalletUsageCount
        };

        var processingStatusWithoutAmount = new List<OrderProcessingStatus>
        {
            OrderProcessingStatus.DepositInstructionCreated, OrderProcessingStatus.RsnReferenceCreated
        };

        if (mintingResult.ProcessingStatus.HasValue
            && processingStatusWithoutAmount.TrueForAll(x => x != mintingResult.ProcessingStatus)
            && mintingResult.MintingAmount is not null)
        {
            mintingDetails.MintingAmount = new MoneyDto
            {
                Amount = mintingResult.MintingAmount.Amount, AssetSymbol = mintingResult.MintingAmount.Currency
            };
        }

        return mintingDetails;
    }
    
    private static List<MintProgressOverview> GetProgressOverview(Order order)
    {
        return
        [
            new MintProgressOverview
            {
                Title = MintProgressOverviewTitle.Payment,
                Status = GetPaymentProcessStatus(order),
                Amount = order.DepositedAmount == null
                    ? null
                    : new MoneyDto
                    {
                        Amount = order.DepositedAmount.Amount,
                        AssetSymbol = order.DepositedAmount.Currency
                    }
            },
            new MintProgressOverview
            {
                Title = MintProgressOverviewTitle.Mint,
                Status = GetMintProgressStatus(order),
                Amount = order.ActualAmount == null
                    ? null
                    : new MoneyDto
                    {
                        Amount = order.ActualAmount.Amount,
                        AssetSymbol = order.ActualAmount.Currency
                    }
            }
        ];
    }

    private static MintProgressOverviewStatus GetPaymentProcessStatus(Order order)
    {
        var upcomingDepositState = new List<OrderProcessingStatus>
        {
            OrderProcessingStatus.DepositInstructionCreated,
            OrderProcessingStatus.RsnReferenceCreated
        };
        if (upcomingDepositState.Exists(x => x == order.ProcessingStatus))
            return MintProgressOverviewStatus.InProgress;
        
        var completeDepositState = new List<OrderProcessingStatus>
        {
            OrderProcessingStatus.DepositInstructionCompleted,
            OrderProcessingStatus.RsnAmountAdded,
            OrderProcessingStatus.SigningInitiated,
            OrderProcessingStatus.TransactionExecuted,
        };
        if (completeDepositState.Exists(x => x == order.ProcessingStatus))
            return MintProgressOverviewStatus.Completed;

        return MintProgressOverviewStatus.Incomplete;
    }

    private static MintProgressOverviewStatus GetMintProgressStatus(Order order)
    {
        return order.ProcessingStatus switch
        {
            OrderProcessingStatus.SigningInitiated => MintProgressOverviewStatus.InProgress,
            OrderProcessingStatus.TransactionExecuted => MintProgressOverviewStatus.Completed,
            _ => MintProgressOverviewStatus.Incomplete
        };
    }
}
