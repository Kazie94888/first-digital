using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.ClientPortal.Api.Base.Dto;
using SmartCoinOS.Domain.Orders;
using SmartCoinOS.Persistence;

namespace SmartCoinOS.ClientPortal.Api.Features.OrderInsights.Details;

internal class RedeemOrderDetailsQueryHandler : IQueryHandler<RedeemOrderDetailsQuery, RedeemOrderDetailsResponse>
{
    private readonly ReadOnlyDataContext _context;

    public RedeemOrderDetailsQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<RedeemOrderDetailsResponse>> Handle(RedeemOrderDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var clientId = request.UserInfo.ClientId;
        var order = await _context.Orders.FirstAsync(o => o.Id == request.OrderId && o.ClientId == clientId,
            cancellationToken);

        var response = new RedeemOrderDetailsResponse
        {
            Overview = GetProgressOverview(order),
            Summary = await GetOrderSummaryAsync(order.Id, cancellationToken),
            DepositDetails = await GetRedeemDepositDetailsAsync(request.OrderId, cancellationToken),
            BurningDetails = await GetRedeemBurningDetailsAsync(request.OrderId, cancellationToken)
        };

        if (order.IsBankTransfer())
            response.BankDetails = await GetBankDetailsAsync(request.OrderId, cancellationToken);
        else
            response.RsnDetails = await GetRsnDetailsAsync(request.OrderId, cancellationToken);

        return Result.Ok(response);
    }

    private async Task<RedeemOrderSummary> GetOrderSummaryAsync(OrderId orderId,
        CancellationToken cancellationToken)
    {
        var queryResult = await (
                from order in _context.Orders
                join client in _context.Clients on order.ClientId equals client.Id
                join entityParticular in _context.EntityParticulars on client.Id equals entityParticular.ClientId
                join orderWallet in _context.Wallets on order.WalletId equals orderWallet.Id
                where order.Id == orderId && !entityParticular.Archived
                select new RedeemOrderSummary
                {
                    OrderNumber = order.OrderNumber.Value,
                    OrderStatus = order.Status,
                    OrderProcessingStatus = order.ProcessingStatus,
                    ClientId = order.ClientId,
                    ClientName = entityParticular.LegalEntityName,
                    ClientStatus = client.Status,
                    OrderedAmount = new MoneyDto
                    {
                        Amount = order.OrderedAmount.Amount, AssetSymbol = order.OrderedAmount.Currency
                    },
                    Network = orderWallet.WalletAccount.Network,
                })
            .FirstAsync(cancellationToken);

        return queryResult;
    }

    private async Task<RedeemDepositDetails> GetRedeemDepositDetailsAsync(OrderId orderId,
        CancellationToken cancellationToken)
    {
        var queryResult = await (
            from order in _context.Orders
            join orderWallet in _context.Wallets on order.WalletId equals orderWallet.Id
            join depositWallet in _context.DepositWallets on order.DepositWalletId equals depositWallet.Id
            where order.Id == orderId
            select new RedeemDepositDetails
            {
                TransactionHash = order.RedeemTxHash,
                Network = orderWallet.WalletAccount.Network,
                FromAddress = orderWallet.WalletAccount.Address,
                ToAddress = depositWallet.Account.Address
            }).FirstAsync(cancellationToken);

        return queryResult;
    }

    private async Task<RedeemBurningDetails> GetRedeemBurningDetailsAsync(OrderId orderId,
        CancellationToken cancellationToken)
    {
        var queryResult = await (
            from order in _context.Orders
            join orderWallet in _context.Wallets on order.WalletId equals orderWallet.Id
            where order.Id == orderId
            select new RedeemBurningDetails
            {
                DepositAmount = new MoneyDto
                {
                    Amount = order.OrderedAmount.Amount, AssetSymbol = order.OrderedAmount.Currency
                },
                BurningAmount = order.ActualAmount == null
                    ? null
                    : new MoneyDto { Amount = order.ActualAmount.Amount, AssetSymbol = order.ActualAmount.Currency },
                FromWallet =
                    new BlockchainAddressDto
                    {
                        Address = orderWallet.WalletAccount.Address, Network = orderWallet.WalletAccount.Network,
                    },
                FromWalletUsageCount = orderWallet.OrdersCount,
                TransactionHash = order.RedeemTxHash,
            }).FirstAsync(cancellationToken);

        return queryResult;
    }

    private async Task<RedeemBankDetails> GetBankDetailsAsync(OrderId orderId, CancellationToken cancellationToken)
    {
        var queryResult = await (
                from order in _context.Orders
                join orderBank in _context.BankAccounts on order.BankAccountId equals orderBank.Id
                join orderBankAddress in _context.Addresses on orderBank.Address.Id equals orderBankAddress.Id
                join depositBank in _context.DepositBanks on order.DepositBankId equals depositBank.Id
                where order.Id == orderId
                select new { OrderBank = orderBank, OrderBankAddress = orderBankAddress, DepositBank = depositBank })
            .FirstAsync(cancellationToken);

        return new RedeemBankDetails
        {
            FromBankAccountName = queryResult.DepositBank.Name,
            FromIban = queryResult.DepositBank.Iban,
            ToBankAccountName = queryResult.OrderBank.BankName,
            ToSwift = queryResult.OrderBank.SwiftCode,
            ToIban = queryResult.OrderBank.Iban,
            ToBankBranchAddress = queryResult.OrderBankAddress.GetFullAddress()
        };
    }

    private async Task<RedeemRsnDetails> GetRsnDetailsAsync(OrderId orderId, CancellationToken cancellationToken)
    {
        var queryResult = await (
            from order in _context.Orders
            join orderFdtDepositAccount in _context.FdtDepositAccounts on order.DepositFdtAccountId equals
                orderFdtDepositAccount.Id
            join orderFdtAccount in _context.FdtAccounts on order.FdtAccountId equals orderFdtAccount.Id
            where order.Id == orderId
            select new RedeemRsnDetails
            {
                FromFdtAccount =
                    new FdtAccountDto
                    {
                        Name = orderFdtDepositAccount.AccountName, Number = orderFdtDepositAccount.AccountNumber
                    },
                ToFdtAccount =
                    new FdtAccountDto { Name = orderFdtAccount.ClientName, Number = orderFdtAccount.AccountNumber, },
                TransactionReferenceNumber = order.RsnReference != null ? order.RsnReference.TransactionId : null
            }).FirstAsync(cancellationToken);

        return queryResult;
    }
    
    private static List<RedeemProgressOverview> GetProgressOverview(Order order)
    {
        return
        [
            new RedeemProgressOverview
            {
                Title = RedeemProgressOverviewTitle.TransferTokens,
                Status = GetTransferTokensProgressStatus(order),
                Amount = order.DepositedAmount == null
                    ? null
                    : new MoneyDto
                    {
                        Amount = order.DepositedAmount.Amount,
                        AssetSymbol = order.DepositedAmount.Currency
                    }
            },
            new RedeemProgressOverview
            {
                Title = RedeemProgressOverviewTitle.Burn,
                Status = GetBurnProgressStatus(order),
                Amount = order.ActualAmount == null
                    ? null
                    : new MoneyDto
                    {
                        Amount = order.ActualAmount.Amount,
                        AssetSymbol = order.ActualAmount.Currency
                    }
            },
            new RedeemProgressOverview
            {
                Title = RedeemProgressOverviewTitle.ReceiptOfFunds,
                Status = GetReceiptOfFundsProgressStatus(order),
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

    private static RedeemProgressOverviewStatus GetTransferTokensProgressStatus(Order order)
    {
        if (order.ProcessingStatus == OrderProcessingStatus.DepositTxHashSet)
            return RedeemProgressOverviewStatus.InProgress;

        var completeStates = new List<OrderProcessingStatus>
        {
            OrderProcessingStatus.TokenDepositConfirmed,
            
            OrderProcessingStatus.PaymentInstructionCreated,
            OrderProcessingStatus.PaymentInstructionCompleted,
            OrderProcessingStatus.RsnReferenceCreated,
            OrderProcessingStatus.WithdrawalConfirmed,
            
            OrderProcessingStatus.SigningInitiated,
            OrderProcessingStatus.TransactionExecuted,
        };
        if (completeStates.Exists(x => x == order.ProcessingStatus))
            return RedeemProgressOverviewStatus.Completed;
        
        return RedeemProgressOverviewStatus.Incomplete;
    }

    private static RedeemProgressOverviewStatus GetBurnProgressStatus(Order order)
    {
        if (order.ProcessingStatus == OrderProcessingStatus.SigningInitiated)
            return RedeemProgressOverviewStatus.InProgress;

        var completeStates = new List<OrderProcessingStatus>
        {
            OrderProcessingStatus.PaymentInstructionCreated,
            OrderProcessingStatus.PaymentInstructionCompleted,
            OrderProcessingStatus.RsnReferenceCreated,
            OrderProcessingStatus.WithdrawalConfirmed,
            
            OrderProcessingStatus.TransactionExecuted,
        };
        if (completeStates.Exists(x => x == order.ProcessingStatus))
            return RedeemProgressOverviewStatus.Completed;
        
        return RedeemProgressOverviewStatus.Incomplete;
    }

    private static RedeemProgressOverviewStatus GetReceiptOfFundsProgressStatus(Order order)
    {
        return order.ProcessingStatus switch
        {
            OrderProcessingStatus.RsnReferenceCreated => RedeemProgressOverviewStatus.InProgress,
            OrderProcessingStatus.WithdrawalConfirmed => RedeemProgressOverviewStatus.Completed,
            _ => RedeemProgressOverviewStatus.Incomplete
        };
    }
}
