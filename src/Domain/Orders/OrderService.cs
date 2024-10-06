using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Deposit;
using SmartCoinOS.Domain.Exceptions;
using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Orders;

public sealed class OrderService
{
    private readonly IOrderNumberService _orderNumberService;

    public OrderService(IOrderNumberService orderNumberService)
    {
        _orderNumberService = orderNumberService;
    }

    public async Task<MintOrder> CreateMintOrderAsync(ClientOrderData clientData,
        FdtOrderData fdtData, Money orderAmount, UserInfo createdBy,
        CancellationToken cancellationToken)
    {
        CreateOrderGuard(clientData, orderAmount);

        var orderNumber = await _orderNumberService.GetOrderNumberAsync(OrderType.Mint, cancellationToken);
        EntityNullException.ThrowIfNull(orderNumber, nameof(orderNumber));

        var order = MintOrder.Create(clientData.ClientId, orderNumber, orderAmount, clientData.WalletId,
            fdtData.WalletId, createdBy);

        SetPaymentMethods(order, clientData.BankAccountId, clientData.FdtAccountId, fdtData.BankId,
            fdtData.FdtAccountId);

        return order;
    }

    public async Task<RedeemOrder> CreateRedeemOrderAsync(ClientOrderData clientData, FdtOrderData fdtData,
        Money redeemAmount,
        UserInfo createdBy,
        CancellationToken cancellationToken)
    {
        CreateOrderGuard(clientData, redeemAmount);

        var orderNumber = await _orderNumberService.GetOrderNumberAsync(OrderType.Redeem, cancellationToken);
        EntityNullException.ThrowIfNull(orderNumber, nameof(orderNumber));

        var order = RedeemOrder.Create(clientData.ClientId, orderNumber, redeemAmount, clientData.WalletId,
            fdtData.WalletId, createdBy);

        SetPaymentMethods(order, clientData.BankAccountId, clientData.FdtAccountId, fdtData.BankId,
            fdtData.FdtAccountId);

        return order;
    }

    private static void SetPaymentMethods(Order order, BankAccountId? clientBank, FdtAccountId? clientFdtAccount,
        DepositBankId? depositBank, DepositFdtAccountId? depositFdtAccount)
    {
        if (clientFdtAccount.HasValue && depositFdtAccount.HasValue)
            order.SetPaymentMode(clientFdtAccount.Value, depositFdtAccount.Value);
        else if (clientBank.HasValue && depositBank.HasValue)
            order.SetPaymentMode(clientBank.Value, depositBank.Value);
        else
            throw new CreateEntityException("Payment mode couldn't be determined.");
    }

    private static void CreateOrderGuard(ClientOrderData clientData, Money orderAmount)
    {
        if (orderAmount.Amount <= 0)
            throw new CreateEntityException("Cannot mint this amount");

        if (clientData.ClientStatus is not ClientStatus.Active)
            throw new CreateEntityException("Client status does not permit order creation");
    }
}

public sealed record ClientOrderData
{
    public required ClientId ClientId { get; init; }
    public required ClientStatus ClientStatus { get; init; }
    public BankAccountId? BankAccountId { get; init; }
    public FdtAccountId? FdtAccountId { get; init; }
    public required WalletId WalletId { get; init; }
}

public sealed record FdtOrderData
{
    public DepositBankId? BankId { get; init; }
    public DepositWalletId? WalletId { get; init; }
    public DepositFdtAccountId? FdtAccountId { get; init; }
}
