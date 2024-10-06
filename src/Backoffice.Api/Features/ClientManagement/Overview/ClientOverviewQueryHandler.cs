using System.Globalization;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Deposit;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Overview;

internal sealed class ClientOverviewQueryHandler : IQueryHandler<ClientOverviewQuery, ClientOverviewResponse>
{
    private readonly ReadOnlyDataContext _context;

    public ClientOverviewQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<ClientOverviewResponse>> Handle(ClientOverviewQuery request, CancellationToken cancellationToken)
    {
        var client = await GetClientAsync(request.ClientId, cancellationToken);
        var depositBank = await GetClientDepositBankAsync(client, cancellationToken);

        var entityParticular = client.CurrentEntityParticular;
        var address = client.Address;
        var atleastOneBankVerified = client.BankAccounts.Any(b => b.VerificationStatus == EntityVerificationStatus.Verified);
        var atleastOneWalletVerified = client.Wallets.Any(b => b.VerificationStatus == EntityVerificationStatus.Verified);


        var overviewResponseDto = new ClientOverviewResponse
        {
            LegalEntityName = entityParticular.LegalEntityName,
            JurisdictionOfInc = entityParticular.CountryOfInc,
            RegistrationNumber = entityParticular.RegistrationNumber,
            DateOfInc = entityParticular.DateOfIncorporation,
            RegistrationAddress = address.GetFullAddress(),
            EntityParticularsVerified = entityParticular.IsVerified(),
            WalletsVerified = atleastOneWalletVerified,
            BankAccountsVerified = atleastOneBankVerified,
            DepositBank = depositBank
        };

        return Result.Ok(overviewResponseDto);
    }

    private async Task<Client> GetClientAsync(ClientId id, CancellationToken cancellationToken)
    {
        var client = await (from c in _context.Clients
                            .Include(b => b.EntityParticulars.Where(ep => !ep.Archived))
                            .Include(a => a.Address)
                            .Include(x => x.Wallets.Where(w => w.VerificationStatus == EntityVerificationStatus.Verified))
                            .Include(x => x.BankAccounts.Where(b => b.VerificationStatus == EntityVerificationStatus.Verified))

                            where c.Id == id
                            select c).FirstAsync(cancellationToken);

        return client;
    }

    private async Task<DepositBankOverview?> GetClientDepositBankAsync(Client client, CancellationToken cancellationToken)
    {
        if (!client.DepositBankId.HasValue
            || !DepositBankId.TryParse(client.DepositBankId.ToString(), CultureInfo.InvariantCulture, out var depositBankId))
            return null;

        var depositBank = await (from db in _context.DepositBanks
                                 where db.Id == depositBankId
                                 select new DepositBankOverview
                                 {
                                     Id = db.Id,
                                     BankName = db.Name,
                                     Iban = db.Iban
                                 }).FirstAsync(cancellationToken);

        return depositBank;
    }
}