using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Deposit;
using SmartCoinOS.Domain.Enums;
using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Tests.Domain.DepositTests;

public abstract class BaseDepositTest
{
    protected const string FdtDepositAccountName = "FDT Deposit Master Account";
    protected const string FdtDepositAccountNumber = "123456789012";

    internal static readonly UserInfo UserInfo = new()
    {
        Id = Guid.NewGuid(), Type = UserInfoType.BackOffice, Username = "Base Deposit Tester"
    };

    internal static readonly AddressRecord SampleAddress = new()
    {
        Country = "United Kingdom", City = "West Lornaport", PostalCode = "97048", Street = "1121 1st street"
    };

    internal static DepositBank GetDefaultDepositBank()
    {
        return GetDepositBankFromFactory(isDefault: true);
    }

    internal static DepositBank GetDepositBank()
    {
        return GetDepositBankFromFactory(isDefault: false);
    }

    private static DepositBank GetDepositBankFromFactory(bool isDefault)
    {
        var deposit = DepositBank.Create("Intesa Sanpaolo",
            "FDT",
            "NIXYT66",
            "IT011234875912EUR",
            SampleAddress,
            isDefault,
            UserInfo);

        return deposit;
    }

    internal static DepositWallet GetDefaultDepositWallet()
    {
        return GetDepositWalletFromFactory(isDefault: true);
    }

    internal static DepositWallet GetDepositWallet()
    {
        return GetDepositWalletFromFactory(isDefault: false);
    }

    private static DepositWallet GetDepositWalletFromFactory(bool isDefault)
    {
        var defaultAddress = "0xacec2a18a244c53114ba27c7dfc9ffbbee14abb7";
        var defaultNetwork = BlockchainNetwork.BnbSmartchain;

        var deposit = DepositWallet.Create(new WalletAccount(defaultAddress, defaultNetwork), isDefault, UserInfo);

        return deposit;
    }

    public static DepositFdtAccount GetFdtDepositAccountFromFactory()
    {
        var accountNumber = new FdtDepositAccountNumber(FdtDepositAccountNumber);
        var fdtDepositAccount = DepositFdtAccount.Create(FdtDepositAccountName,
            accountNumber, UserInfo);

        return fdtDepositAccount;
    }
}
