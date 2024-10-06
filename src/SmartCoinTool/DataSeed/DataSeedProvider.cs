using Bogus;
using SmartCoinOS.Domain;
using SmartCoinOS.Domain.Application;
using SmartCoinOS.Domain.AuditLogs;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Deposit;
using SmartCoinOS.Domain.Enums;
using SmartCoinOS.Domain.Orders;
using SmartCoinOS.Domain.Orders.Events;
using SmartCoinOS.Domain.SeedWork;
using SmartCoinOS.Domain.Shared;

namespace SmartCoinOS.SmartCoinTool.DataSeed;

internal static class DataSeedProvider
{
    /// <summary>
    ///     The reference date is very important! Without it, it will generate a random date based on the CURRENT date on the
    ///     system.
    ///     Generating a date based on the system date is not deterministic!
    ///     So the solution is to pass in a constant date instead which will be used to generate a random date
    /// </summary>
    private static readonly DateTimeOffset _referenceDate = new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);

    private static readonly UserInfo _userInfo = new()
    {
        Id = Guid.NewGuid(),
        Type = UserInfoType.BackOffice,
        Username = "Seeder"
    };

    private const int _seedValue = 12345;
    private const int _clientEntitiesMin = 1;
    private const int _clientEntitiesMax = 7;
    
    public static string GetRandomCountry()
    {
        var supportedCountries = GlobalConstants.Country.SupportedCountries().ToArray();
        return supportedCountries[Randomizer.Seed.Next(0, supportedCountries.Length)];
    }

    static DataSeedProvider()
    {
        Randomizer.Seed = new Random(_seedValue);
    }

    public static List<Client> GenerateClients(int clientCount, List<DepositBank> depositBanks,
        Stack<KeyValuePair<string, string>> knownUsers)
    {
        var clientList = new List<Client>();
        foreach (var iteration in Enumerable.Range(0, clientCount))
        {
            var entityParticular = GetEntityParticularFaker().Generate();
            var address = GetAddressFaker().Generate();
            var appFaker = new Faker();
            var appId = appFaker.Random.AlphaNumeric(8);

            var clientFaker = new Faker<Client>().CustomInstantiator(f =>
            {
                var builder = new ClientBuilder(_userInfo);
                builder.AddEntityParticulars(entityParticular.LegalEntityName, entityParticular.RegistrationNumber,
                    entityParticular.DateOfIncorporation,
                    CompanyLegalStructure.Partnership,
                    null,
                    entityParticular.CountryOfInc);

                builder.AddAddress(address.Country, state: null, address.PostalCode, address.City,
                    address.Street);
                builder.AddContact(f.Internet.Email(), f.Phone.PhoneNumber());
                builder.AddMetadata(appId, f.PickRandom(depositBanks).Id);

                var client = builder.Build();
                return client;
            });

            var client = clientFaker.Generate();

            GetWalletFaker(client.Id)
                .GenerateBetween(_clientEntitiesMin, _clientEntitiesMax)
                .ForEach(w => client.AddWallet(w.WalletAccount.Address, w.WalletAccount.Network, w.Alias, _userInfo));

            GetFdtAccountFaker(client.Id)
                .GenerateBetween(_clientEntitiesMin, _clientEntitiesMax)
                .ForEach(f =>
                {
                    var fdtAccount = client.AddFdtAccount(f.ClientName, f.AccountNumber, f.Alias, f.CreatedBy);
                    switch (f.VerificationStatus)
                    {
                        case EntityVerificationStatus.Unverified:
                            break;

                        case EntityVerificationStatus.Verified:
                            client.VerifyFdtAccount(fdtAccount.Value.Id, _userInfo);
                            break;

                        case EntityVerificationStatus.Declined:
                            client.DeclineFdtAccount(fdtAccount.Value.Id, _userInfo);
                            break;
                    }
                });

            GetBankAccountFaker(client.Id)
                .GenerateBetween(_clientEntitiesMin, _clientEntitiesMax)
                .ForEach(b =>
                {
                    var addr = b.Address;
                    var addressRecord = new AddressRecord()
                    {
                        Country = addr.Country,
                        State = addr.State,
                        City = addr.City,
                        PostalCode = addr.PostalCode,
                        Street = addr.Street
                    };

                    var bankAccountRecord = new CreatedClientBankAccountRecord
                    {
                        Beneficiary = b.Beneficiary,
                        Iban = b.Iban,
                        BankName = b.BankName,
                        SwiftCode = b.SwiftCode,
                        Alias =  b.Alias,
                        SmartTrustBank = b.SmartTrustBank,
                        Address = addressRecord
                    };
                    
                    client.AddBankAccount(bankAccountRecord, _userInfo);
                });

            int minUsers = 50,
                maxUsers = 80;

            GetAuthorizedUser(client.Id, iteration)
                .GenerateBetween(minUsers, maxUsers)
                .ForEach(u =>
                {
                    if (knownUsers.Count > 0)
                    {
                        var (adId, adEmail) = knownUsers.Pop();
                        client.AddAuthorizedUser(u.FirstName, u.LastName, adEmail, adId, _userInfo);
                    }
                    else
                        client.AddAuthorizedUser(u.FirstName, u.LastName, u.Email, u.ExternalId, _userInfo);
                });

            clientList.Add(client);
        }

        return clientList;
    }

    public static IReadOnlyCollection<Order> GenerateOrders(
        FakeOrderNumberGenerator fakeOrderNumberGenerator,
        ClientId clientId,
        IReadOnlyList<WalletId> walletIds,
        IReadOnlyList<BankAccountId> bankAccountIds,
        IReadOnlyList<FdtAccountId> fdtAccountIds,
        DepositBankId? depositBank,
        DepositFdtAccountId? depositFdt,
        DepositWalletId depositWallet,
        int maxOrderCount)
    {
        var orders = new List<Order>(maxOrderCount);

        var orderFaker = GetOrderFaker(fakeOrderNumberGenerator, clientId, walletIds, bankAccountIds, fdtAccountIds,
            depositBank, depositFdt, depositWallet);

        orders.AddRange(orderFaker.Generate(maxOrderCount));
        return orders;
    }

    public static IReadOnlyCollection<AuditLog> GenerateAuditLogs(ClientId clientId, int maxAuditLogCount)
    {
        var auditLogs = GetAuditLogFaker(clientId).Generate(maxAuditLogCount);
        return auditLogs;
    }

    public static IReadOnlyCollection<AuditLog> GenerateOrderAuditLogs(ClientId clientId,
        IReadOnlyCollection<Order> orders, int countOfLines)
    {
        var capacity = countOfLines * orders.Count;
        var auditLogs = new List<AuditLog>(capacity);

        foreach (var order in orders)
        {
            var logs = GetOrderAuditLogFaker(clientId, order).Generate(countOfLines);
            auditLogs.AddRange(logs);
        }

        return auditLogs;
    }

    public static IReadOnlyCollection<DepositBank> GenerateDepositBanks()
    {
        const int generateInactiveCount = 5;

        var activeDepositBank = GetDepositBankFaker(true).Generate();
        var inActiveDepositBanks = GetDepositBankFaker(false).Generate(generateInactiveCount);

        var depositBanks = new List<DepositBank> { activeDepositBank };
        depositBanks.AddRange(inActiveDepositBanks);
        return depositBanks;
    }

    public static IReadOnlyCollection<DepositWallet> GenerateDepositWallets()
    {
        const int generateTotalCount = 1;

        var depositWallets = GetDepositWalletFaker(true).Generate(generateTotalCount);
        return depositWallets;
    }

    private static Faker<Order> GetOrderFaker(
        FakeOrderNumberGenerator fakeOrderNumberGenerator,
        ClientId clientId,
        IReadOnlyList<WalletId> walletIds,
        IReadOnlyList<BankAccountId> bankAccountIds,
        IReadOnlyList<FdtAccountId> fdtAccountIds,
        DepositBankId? depositBank,
        DepositFdtAccountId? depositFdt,
        DepositWalletId depositWallet)
    {
        var orderService = new OrderService(fakeOrderNumberGenerator);

        return new Faker<Order>()
            .CustomInstantiator(f =>
            {
                var orderType = f.PickRandom<OrderType>();
                var walletId = f.PickRandom<WalletId>(walletIds);
                var bankAccountId = f.PickRandom<BankAccountId>(bankAccountIds);
                FdtAccountId? fdtAccountId = fdtAccountIds.Any() ? f.PickRandom<FdtAccountId>(fdtAccountIds) : null;

                var orderAmount = new Money
                {
                    Amount = decimal.Round(f.Random.Decimal(1_000_000, 15_000_000), 4),
                    Currency = GlobalConstants.Currency.Fdusd
                };

                var clientOrderData = new ClientOrderData()
                {
                    ClientId = clientId,
                    WalletId = walletId,
                    BankAccountId = bankAccountId,
                    FdtAccountId = fdtAccountId,
                    ClientStatus = ClientStatus.Active
                };

                var fdtOrderData = new FdtOrderData()
                {
                    FdtAccountId = depositFdt,
                    WalletId = depositWallet,
                    BankId = depositBank
                };


                Order order = orderType switch
                {
                    OrderType.Mint => orderService.CreateMintOrderAsync(clientOrderData, fdtOrderData, orderAmount,
                        _userInfo, CancellationToken.None).Result,
                    _ => orderService.CreateRedeemOrderAsync(clientOrderData, fdtOrderData, orderAmount,
                        _userInfo, CancellationToken.None).Result
                };

                order.CreatedAt = f.Date.PastOffset(refDate: _referenceDate);
                return order;
            });
    }

    private static Faker<EntityParticular> GetEntityParticularFaker()
    {
        return new Faker<EntityParticular>()
            .CustomInstantiator(f =>
            {
                var dateTimeOffset = f.Date.PastOffset(refDate: _referenceDate);
                return new EntityParticular
                {
                    Id = new EntityParticularId(f.Random.Guid()),
                    LegalEntityName = f.Company.CompanyName(),
                    RegistrationNumber = f.Random.AlphaNumeric(10),
                    CountryOfInc = f.Address.County(),
                    LegalStructure = f.PickRandom<CompanyLegalStructure>(),
                    DateOfIncorporation =
                        new DateOnly(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day),
                    CreatedBy = _userInfo
                };
            });
    }

    private static Faker<Address> GetAddressFaker()
    {
        return new Faker<Address>()
            .CustomInstantiator(f => new Address
            {
                Id = new AddressId(f.Random.Uuid()),
                Country = GetRandomCountry(),
                City = f.Address.City(),
                PostalCode = f.Address.ZipCode(),
                Street = f.Address.StreetAddress(),
                CreatedBy = _userInfo
            });
    }

    private static Faker<Wallet> GetWalletFaker(ClientId clientId)
    {
        return new Faker<Wallet>()
            .CustomInstantiator(f => new Wallet
            {
                Id = new WalletId(f.Random.Guid()),
                ClientId = clientId,
                WalletAccount = new WalletAccount(f.Finance.EthereumAddress(), f.PickRandom<BlockchainNetwork>()),
                CreatedBy = _userInfo,
                Alias = f.Internet.UserName()
            });
    }

    private static Faker<FdtAccount> GetFdtAccountFaker(ClientId clientId)
    {
        return new Faker<FdtAccount>()
            .CustomInstantiator(f =>
            {
                var randomNumber = f.Random.Number(1_000_000, 9_000_000).ToString();
                var accountNumber = $"{randomNumber}".PadLeft(12, '0');
                var fdtAccount = new FdtAccount
                {
                    Id = new FdtAccountId(f.Random.Guid()),
                    ClientId = clientId,
                    ClientName = f.Company.CompanyName(),
                    AccountNumber = new FdtAccountNumber(accountNumber),
                    Alias = f.Random.Words(2),
                    VerificationStatus = f.PickRandom<EntityVerificationStatus>(),
                    CreatedBy = _userInfo
                };

                return fdtAccount;
            });
    }

    private static Faker<BankAccount> GetBankAccountFaker(ClientId clientId)
    {
        // this is a known value. if we add here a random number, then it will not be possible to create orders from this bank
        const int smartTrustBankId = 9690;

        return new Faker<BankAccount>()
            .CustomInstantiator(f =>
            {
                var bankAccount = new BankAccount
                {
                    Id = new BankAccountId(f.Random.Guid()),
                    ClientId = clientId,
                    Beneficiary = f.Name.FullName(),
                    Iban = f.Finance.Iban(),
                    BankName = f.Company.CompanyName(),
                    SwiftCode = f.Finance.Bic(),
                    Address = GetAddressFaker().Generate(),
                    CreatedBy = _userInfo,
                    SmartTrustBank = new SmartTrustBank(smartTrustBankId, smartTrustBankId)
                };
                bankAccount.SetAlias(f.Name.FullName());
                
                return bankAccount;
            });
    }

    private static Faker<AuthorizedUser> GetAuthorizedUser(ClientId clientId, int batch)
    {
        return new Faker<AuthorizedUser>()
            .CustomInstantiator(f =>
            {
                var firstName = f.Name.FirstName();
                var lastName = f.Name.LastName();

                return new AuthorizedUser
                {
                    Id = new AuthorizedUserId(f.Random.Uuid()),
                    ClientId = clientId,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = f.Internet.Email(firstName: firstName, lastName: lastName, uniqueSuffix: $"-{batch}"),
                    ExternalId = f.Random.Uuid().ToString(),
                    CreatedBy = _userInfo
                };
            });
    }

    private static Faker<AuditLog> GetOrderAuditLogFaker(ClientId clientId, Order order)
    {
        return new Faker<AuditLog>()
            .CustomInstantiator(f =>
            {
                var action = f.PickRandom(OrderAuditEventBase.OrderEventNames);
                var timestamp = f.Date.PastOffset(refDate: _referenceDate);
                var description = f.Lorem.Sentence(3);

                var parameters = new List<AuditLogParameter>
                {
                    new(GlobalConstants.AuditParameters.ClientId, clientId.Value.ToString()),
                    new(nameof(OrderId), order.Id.ToString()),
                    new(nameof(Order.Type), order.Type.ToString()),
                    new(nameof(Order.OrderNumber), order.OrderNumber.ToString())
                };

                return AuditLog.Create(action, description, parameters, _userInfo, timestamp);
            });
    }

    private static Faker<AuditLog> GetAuditLogFaker(ClientId clientId)
    {
        return new Faker<AuditLog>()
            .CustomInstantiator(f =>
            {
                var audits = (from evt in AssemblyReference.Assembly.GetTypes()
                    where evt.IsClass
                          && !evt.IsAbstract
                          && evt.IsSubclassOf(typeof(AuditEventBase))
                    select evt.Name).ToList();


                var action = f.PickRandom(audits);
                var timestamp = f.Date.PastOffset(refDate: _referenceDate);
                var description = f.Lorem.Sentence();

                var parameters = new List<AuditLogParameter>
                {
                    new(GlobalConstants.AuditParameters.ClientId, clientId.Value.ToString()),
                    new(f.Lorem.Slug(), f.Lorem.Word()),
                    new(f.Lorem.Slug(), f.Lorem.Word()),
                    new(f.Lorem.Slug(), f.Lorem.Word()),
                    new(f.Lorem.Slug(), f.Lorem.Word()),
                    new(f.Lorem.Slug(), f.Lorem.Word()),
                    new(f.Lorem.Slug(), f.Lorem.Word())
                };

                return AuditLog.Create(action, description, parameters, _userInfo, timestamp);
            });
    }

    private static Faker<DepositBank> GetDepositBankFaker(bool isDefault)
    {
        return new Faker<DepositBank>()
            .CustomInstantiator(f =>
            {
                var addr = GetAddressFaker().Generate();
                var addressRecord = new AddressRecord()
                {
                    Country = addr.Country,
                    State = addr.State,
                    City = addr.City,
                    PostalCode = addr.PostalCode,
                    Street = addr.Street
                };

                return DepositBank.Create(f.Company.CompanyName(), f.Name.FullName(), f.Finance.Bic(), f.Finance.Iban(),
                    addressRecord, isDefault, _userInfo);
            });
    }

    private static Faker<DepositWallet> GetDepositWalletFaker(bool isDefault)
    {
        return new Faker<DepositWallet>()
            .CustomInstantiator(f =>
                DepositWallet.Create(
                    new WalletAccount(f.Finance.EthereumAddress(), f.PickRandom<BlockchainNetwork>()),
                    isDefault,
                    _userInfo));
    }

    public static List<DepositFdtAccount> GenerateFdtDepositAccounts(int numberOfFdtDepositAccounts)
    {
        var fdtDepositAccounts = new List<DepositFdtAccount>();
        foreach (var _ in Enumerable.Range(0, numberOfFdtDepositAccounts))
        {
            var fdtDepositAccountFaker = GetFdtDepositAccountFaker();
            var fdtDepositAccount = fdtDepositAccountFaker.Generate();

            if (Randomizer.Seed.Next(0, 2) == 1)
                fdtDepositAccount.Archive(_userInfo);

            fdtDepositAccounts.Add(fdtDepositAccount);
        }

        return fdtDepositAccounts;
    }

    private static Faker<DepositFdtAccount> GetFdtDepositAccountFaker()
    {
        return new Faker<DepositFdtAccount>().CustomInstantiator(f =>
        {
            var fdtDepositAccountName = f.Company.CompanyName();

            var randomNumber = f.Random.Number(1_000_000, 9_000_000).ToString();
            var fdtDepositAccountNumber = new FdtDepositAccountNumber($"{randomNumber}".PadLeft(12, '0'));

            var fdtDepositAccount = DepositFdtAccount.Create(fdtDepositAccountName, fdtDepositAccountNumber, _userInfo);

            return fdtDepositAccount;
        });
    }
}
