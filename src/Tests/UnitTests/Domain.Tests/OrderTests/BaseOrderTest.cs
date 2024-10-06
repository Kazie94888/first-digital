using NSubstitute;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Deposit;
using SmartCoinOS.Domain.Orders;
using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Tests.Domain.OrderTests;

public abstract class BaseOrderTest
{
    static readonly ClientId _clientId = ClientId.Parse("8c18da97-eb16-4850-a628-ff4f1a62070f");
    static readonly WalletId _walletId = WalletId.Parse("3783c82e-6c4a-4794-b014-f6260288330d");
    static readonly DepositWalletId _depositWallet = DepositWalletId.Parse("3783c82e-6c4a-4850-a01e-98170e755844");
    static readonly FdtAccountId _fdtAccountId = FdtAccountId.Parse("854604ab-bd86-4e62-a01e-98170e755844");
    static readonly DepositFdtAccountId _depositFdt = DepositFdtAccountId.Parse("854604ab-bd86-4e62-a01e-98170e755844");
    static readonly BankAccountId _bankId = BankAccountId.Parse("332604ab-bd86-4e62-a01e-98170e711233");
    static readonly DepositBankId _depositBank = DepositBankId.Parse("246604ab-6c4a-4850-a01e-98170e755844");
    static readonly OrderNumber _orderNumber = OrderNumber.New(OrderType.Mint);

    static readonly Money _orderAmount = new()
    {
        Amount = 100_000,
        Currency = "FDUSD"
    };

    protected static readonly SafeSignature Signature = new("0x123456", "Mega John", new DateTime(2024, 01, 05));

    protected static readonly ClientOrderData ClientData = new()
    {
        ClientId = _clientId,
        WalletId = _walletId,
        BankAccountId = _bankId,
        FdtAccountId = _fdtAccountId,
        ClientStatus = ClientStatus.Active
    };

    protected static readonly FdtOrderData FdtData = new()
    {
        FdtAccountId = _depositFdt,
        WalletId = _depositWallet,
        BankId = _depositBank
    };

    internal static readonly UserInfo UserInfo = new()
    {
        Id = Guid.NewGuid(),
        Type = SmartCoinOS.Domain.Enums.UserInfoType.BackOffice,
        Username = "DomainUnitTest"
    };

    protected static RedeemOrder GetRedeemOrderFromFactory() => CreateRedeemOrder();

    protected static RedeemOrder GetBankRedeemOrderFromFactory() => CreateRedeemOrder(paymentIsRsn: false);

    protected static MintOrder GetMintOrderFromFactory() => CreateMintOrder();
    protected static MintOrder GetBankMintOrderFromFactory() => CreateMintOrder(paymentIsRsn: false);

    private static RedeemOrder CreateRedeemOrder(bool paymentIsRsn = true)
    {
        BankAccountId? bankId = null;
        FdtAccountId? fdtAccountId = null;
        DepositFdtAccountId? depositFdt = null;
        DepositBankId? depositBank = null;

        if (paymentIsRsn)
        {
            fdtAccountId = _fdtAccountId;
            depositFdt = _depositFdt;
        }
        else
        {
            bankId = _bankId;
            depositBank = _depositBank;
        }

        var orderNumberService = Substitute.For<IOrderNumberService>();
        orderNumberService.GetOrderNumberAsync(Arg.Any<OrderType>(), CancellationToken.None).Returns(_orderNumber);
        var orderService = new OrderService(orderNumberService);

        var clientOrderData = ClientData with
        {
            BankAccountId = bankId,
            FdtAccountId = fdtAccountId
        };

        var fdtOrderData = FdtData with
        {
            FdtAccountId = depositFdt,
            BankId = depositBank
        };

        return orderService
            .CreateRedeemOrderAsync(clientOrderData, fdtOrderData, _orderAmount, UserInfo, CancellationToken.None)
            .Result;
    }

    private static MintOrder CreateMintOrder(bool paymentIsRsn = true)
    {
        BankAccountId? bankId = null;
        FdtAccountId? fdtAccountId = null;
        DepositFdtAccountId? depositFdt = null;
        DepositBankId? depositBank = null;

        if (paymentIsRsn)
        {
            fdtAccountId = _fdtAccountId;
            depositFdt = _depositFdt;
        }
        else
        {
            bankId = _bankId;
            depositBank = _depositBank;
        }

        var orderNumberService = Substitute.For<IOrderNumberService>();
        orderNumberService.GetOrderNumberAsync(Arg.Any<OrderType>(), CancellationToken.None).Returns(_orderNumber);
        var orderService = new OrderService(orderNumberService);

        var clientOrderData = ClientData with
        {
            BankAccountId = bankId,
            FdtAccountId = fdtAccountId
        };

        var fdtOrderData = FdtData with
        {
            FdtAccountId = depositFdt,
            BankId = depositBank
        };

        return orderService
            .CreateMintOrderAsync(clientOrderData, fdtOrderData, _orderAmount, UserInfo, CancellationToken.None).Result;
    }
}
