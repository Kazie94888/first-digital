using NSubstitute;
using NSubstitute.ReturnsExtensions;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Deposit;
using SmartCoinOS.Domain.Exceptions;
using SmartCoinOS.Domain.Orders;

namespace SmartCoinOS.Tests.Domain.OrderTests;

public sealed class OrderServiceTests : BaseOrderTest
{
    private readonly IOrderNumberService _orderNumberService = Substitute.For<IOrderNumberService>();
    private readonly ClientId _clientId = ClientId.Parse("8c18da97-eb16-4850-a628-ff4f1a62070f");
    private readonly WalletId _walletId = WalletId.Parse("3783c82e-6c4a-4794-b014-f6260288330d");
    private readonly BankAccountId _bankId = BankAccountId.Parse("4783c82e-6c4a-4794-b014-f6260288330d");
    private readonly DepositBankId _depositBankId = DepositBankId.Parse("5258c82e-eb16-4794-a628-f6260288330d");
    private readonly DepositWalletId _depositWalletId = DepositWalletId.Parse("7958c847-eb16-4794-a628-f6260288330d");

    private readonly DepositFdtAccountId _depositFdtAccountId =
        DepositFdtAccountId.Parse("7958c847-eb16-4794-a628-f6260288330d");

    private readonly Money _amount = new()
    {
        Amount = 100_000,
        Currency = GlobalConstants.Currency.Fdusd
    };

    [Fact]
    public void CreateMintOrderAsync_WhenPaymentMissing_ThrowsException()
    {
        var orderNumber = OrderNumber.New(OrderType.Mint);
        _orderNumberService.GetOrderNumberAsync(Arg.Any<OrderType>(), CancellationToken.None).Returns(orderNumber);
        var orderService = new OrderService(_orderNumberService);

        var clientOrderData = new ClientOrderData()
        {
            ClientId = _clientId,
            WalletId = _walletId,
            ClientStatus = ClientStatus.Active
        };
        var fdtOrderData = new FdtOrderData()
        {
            FdtAccountId = _depositFdtAccountId,
            WalletId = _depositWalletId
        };

        Task<MintOrder> AttemptResult() => orderService.CreateMintOrderAsync(clientOrderData, fdtOrderData, _amount,
            UserInfo,
            CancellationToken.None);

        Assert.ThrowsAsync<CreateEntityException>(AttemptResult);
    }

    [Fact]
    public void CreateMintOrderAsync_WhenAmountInvalid_ThrowsException()
    {
        var orderNumber = OrderNumber.New(OrderType.Mint);
        _orderNumberService.GetOrderNumberAsync(Arg.Any<OrderType>(), CancellationToken.None).Returns(orderNumber);
        var orderService = new OrderService(_orderNumberService);

        var clientOrderData = new ClientOrderData()
        {
            ClientId = _clientId,
            WalletId = _walletId,
            ClientStatus = ClientStatus.Active
        };
        var fdtOrderData = new FdtOrderData()
        {
            FdtAccountId = _depositFdtAccountId,
            WalletId = _depositWalletId
        };

        Task<MintOrder> AttemptResult() => orderService.CreateMintOrderAsync(clientOrderData, fdtOrderData, _amount,
            UserInfo,
            CancellationToken.None);

        Assert.ThrowsAsync<CreateEntityException>(AttemptResult);
    }

    [Fact]
    public void CreateRedeemOrderAsync_WhenPaymentMissing_ThrowsException()
    {
        var orderNumber = OrderNumber.New(OrderType.Mint);
        _orderNumberService.GetOrderNumberAsync(Arg.Any<OrderType>(), CancellationToken.None).Returns(orderNumber);
        var orderService = new OrderService(_orderNumberService);

        var clientOrderData = new ClientOrderData()
        {
            ClientId = _clientId,
            WalletId = _walletId,
            ClientStatus = ClientStatus.Active
        };

        var fdtOrderData = new FdtOrderData()
        {
            FdtAccountId = _depositFdtAccountId,
            WalletId = _depositWalletId,
            BankId = _depositBankId
        };

        Task<RedeemOrder> AttemptResult() => orderService.CreateRedeemOrderAsync(clientOrderData, fdtOrderData, _amount,
            UserInfo, CancellationToken.None);

        Assert.ThrowsAsync<CreateEntityException>(AttemptResult);
    }

    [Fact]
    public void CreateRedeemOrderAsync_WhenAmountInvalid_ThrowsException()
    {
        var orderNumber = OrderNumber.New(OrderType.Mint);
        _orderNumberService.GetOrderNumberAsync(Arg.Any<OrderType>(), CancellationToken.None).Returns(orderNumber);
        var orderService = new OrderService(_orderNumberService);

        var invalidAmount = new Money
        {
            Amount = 0,
            Currency = GlobalConstants.Currency.Fdusd
        };

        Task<RedeemOrder> AttemptResult() => orderService.CreateRedeemOrderAsync(ClientData, FdtData,
            invalidAmount, UserInfo, CancellationToken.None);

        Assert.ThrowsAsync<CreateEntityException>(AttemptResult);
    }

    [Fact]
    public void CreateRedeemOrderAsync_WhenOrderNumberNotCreated_ThrowsException()
    {
        _orderNumberService.GetOrderNumberAsync(Arg.Any<OrderType>(), CancellationToken.None).ReturnsNull();
        var orderService = new OrderService(_orderNumberService);

        Task<RedeemOrder> AttemptResult() => orderService.CreateRedeemOrderAsync(ClientData, FdtData, _amount,
            UserInfo,
            CancellationToken.None);

        Assert.ThrowsAsync<EntityNullException>(AttemptResult);
    }

    [Fact]
    public void CreateRedeemOrderAsync_WhenPaymentInstrumentsNotSet_FailsWithError()
    {
        var orderNumber = OrderNumber.New(OrderType.Redeem);
        _orderNumberService.GetOrderNumberAsync(Arg.Any<OrderType>(), CancellationToken.None).Returns(orderNumber);

        var orderService = new OrderService(_orderNumberService);
        var clientData = new ClientOrderData()
        {
            ClientId = _clientId,
            WalletId = _walletId,
            ClientStatus = ClientStatus.Active
        };

        Task<RedeemOrder> AttemptResult() =>
            orderService.CreateRedeemOrderAsync(clientData, FdtData, _amount, UserInfo, CancellationToken.None);

        Assert.ThrowsAsync<CreateEntityException>(AttemptResult);
    }

    [Fact]
    public void CreateRedeemOrder_WhenOk_PropertiesAreSetAsync()
    {
        var orderNumber = OrderNumber.New(OrderType.Redeem);
        _orderNumberService.GetOrderNumberAsync(Arg.Any<OrderType>(), CancellationToken.None).Returns(orderNumber);

        var orderService = new OrderService(_orderNumberService);

        var clientData = ClientData with { FdtAccountId = null };
        var fdtData = FdtData with { FdtAccountId = null };

        var order = orderService.CreateRedeemOrderAsync(clientData, fdtData,
            _amount, UserInfo, CancellationToken.None).Result;

        Assert.NotNull(order);
        Assert.Equal(_bankId, order.BankAccountId);
        Assert.Equal(_depositBankId, order.DepositBankId);
    }
}
