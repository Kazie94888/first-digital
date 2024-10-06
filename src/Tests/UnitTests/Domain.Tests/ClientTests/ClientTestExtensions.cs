using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Tests.Domain.ClientTests;

internal static class ClientTestExtensions
{
    internal static Client EnrichWithBankAccount(this Client client, bool verified = false)
    {
        var bankAccountRecord = new CreatedClientBankAccountRecord
        {
            Beneficiary = "Johnathan Collins",
            Iban = "XK788198071070721032",
            BankName = "Bins Group",
            SwiftCode = "LIAICNT1",
            Alias = "Nasir McCullough",
            SmartTrustBank = new SmartTrustBank(2850, 2850),
            Address = BaseClientTest.SampleAddress
        };
        var result = client.AddBankAccount(bankAccountRecord, BaseClientTest.UserInfo);

        if (verified)
            client.VerifyBankAccount(result.Value.Id, BaseClientTest.UserInfo);

        return client;
    }

    internal static Client EnrichWithWallet(this Client client, bool verified = false)
    {
        var result = client.AddWallet("0xa794f5ea0ba59494ce839613ffhba74279589269",
            BlockchainNetwork.EthereumMainnet, "Wallet From UnitTest",
            BaseClientTest.UserInfo);

        if (verified)
            client.VerifyWallet(result.Value.Id, BaseClientTest.UserInfo);

        return client;
    }

    internal static Client EnrichWithAuthorizedUser(this Client client)
    {
        client.AddAuthorizedUser(
            "John",
            "Doe",
            "test@gmail.com",
            Guid.NewGuid().ToString(),
            BaseClientTest.UserInfo);
        return client;
    }

    internal static Client EnrichWithFdtAccount(this Client client, bool verified = false)
    {
        var result = client.AddFdtAccount(
            "Bincota",
            "0011010101",
            "Genius Inc",
            BaseClientTest.UserInfo
        );

        if (verified)
            client.VerifyFdtAccount(result.Value.Id, BaseClientTest.UserInfo);

        return client;
    }
}
