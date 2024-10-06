using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Orders;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Create;

internal sealed class CreateOrderClientResourcesQueryHandler : IQueryHandler<CreateOrderClientResourcesQuery, CreateOrderClientResourcesResponse>
{
    private readonly ReadOnlyDataContext _context;

    public CreateOrderClientResourcesQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<CreateOrderClientResourcesResponse>> Handle(CreateOrderClientResourcesQuery query, CancellationToken cancellationToken)
    {
        var client = await _context.Clients
            .Include(c => c.BankAccounts.Where(b => !b.Archived && b.VerificationStatus == EntityVerificationStatus.Verified))
            .Include(c => c.FdtAccounts.Where(f => !f.Archived && f.VerificationStatus == EntityVerificationStatus.Verified))
            .Include(c => c.Wallets.Where(w => !w.Archived && w.VerificationStatus == EntityVerificationStatus.Verified))
            .Where(c => c.Id == query.ClientId && c.Status == ClientStatus.Active)
            .FirstAsync(cancellationToken);

        var clientResources = new CreateOrderClientResourcesResponse
        {
            BankAccounts = ExtractBankAccounts(client, query.OrderType()),
            FdtAccounts = client.FdtAccounts.Select(f => new FdtAccountListItem
            {
                Id = f.Id,
                Account = new FdtAccountDto
                {
                    Name = f.ClientName,
                    Number = f.AccountNumber
                },
                Alias = f.Alias
            }).ToList(),
            Wallets = client.Wallets.Select(w => new WalletListItem
            {
                Id = w.Id,
                Address = new BlockchainAddressDto
                {
                    Address = w.WalletAccount.Address,
                    Network = w.WalletAccount.Network
                },
                Alias = w.Alias
            }).ToList()
        };

        return Result.Ok(clientResources);
    }

    private static List<BankAccountListItem> ExtractBankAccounts(Client client, OrderType type)
    {
        var banksQuery = type == OrderType.Mint
            ? client.BankAccounts.Where(b => b.SmartTrustBank.OwnBankId.HasValue)
            : client.BankAccounts.Where(b => b.SmartTrustBank.ThirdPartyBankId.HasValue);

        return banksQuery.Select(b => new BankAccountListItem
        {
            Id = b.Id,
            Iban = b.Iban,
            Name = b.BankName,
            Alias = b.Alias
        }).ToList();
    }
}