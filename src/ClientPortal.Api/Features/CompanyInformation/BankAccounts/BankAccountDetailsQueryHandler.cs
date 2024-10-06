using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.ClientPortal.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Persistence;

namespace SmartCoinOS.ClientPortal.Api.Features.CompanyInformation.BankAccounts;

internal sealed class
    BankAccountDetailsQueryHandler : IQueryHandler<BankAccountDetailsQuery, BankAccountDetailsResponse>
{
    private readonly ReadOnlyDataContext _context;
    private const int _recentOrderCount = 5;

    public BankAccountDetailsQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<BankAccountDetailsResponse>> Handle(BankAccountDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var clientId = request.UserInfo.ClientId;

        var bankAccountDetailsResponse = new BankAccountDetailsResponse
        {
            Bank = await GetBankDetailsAsync(clientId, request.BankId, cancellationToken),
            RelatedOrders = await GetRelatedOrdersAsync(clientId, request.BankId, cancellationToken)
        };

        return Result.Ok(bankAccountDetailsResponse);
    }

    private async Task<BankAccountDetails> GetBankDetailsAsync(ClientId clientId, BankAccountId bankId,
        CancellationToken cancellationToken)
    {
        var bankAccount = await _context.BankAccounts
            .Include(a => a.Address)
            .Where(x => x.ClientId == clientId && x.Id == bankId)
            .FirstAsync(cancellationToken);

        var bankDetails = new BankAccountDetails
        {
            Id = bankAccount.Id,
            Beneficiary = bankAccount.Beneficiary,
            SwiftCode = bankAccount.SwiftCode,
            BankName = bankAccount.BankName,
            Iban = bankAccount.Iban,
            Alias = bankAccount.Alias,
            Status = bankAccount.VerificationStatus,
            CreatedAt = bankAccount.CreatedAt,
            Archived = bankAccount.Archived,
            ArchivedAt = bankAccount.ArchivedAt,
            Address = bankAccount.Address.GetFullAddress(),
            CreatedBy = new UserInfoDto
            {
                Id = bankAccount.CreatedBy.Id, Username = bankAccount.CreatedBy.Username, ClientId = clientId
            },
        };

        return bankDetails;
    }

    private async Task<List<RelatedOrdersDto>> GetRelatedOrdersAsync(ClientId clientId, BankAccountId bankId,
        CancellationToken cancellationToken)
    {
        var orders = await _context.Orders.Where(x => x.ClientId == clientId && x.BankAccountId == bankId)
            .Select(x => new RelatedOrdersDto
            {
                Id = x.Id,
                Type = x.Type,
                PrettyId = x.OrderNumber.Value,
                CreatedAt = x.CreatedAt,
                Status = x.Status,
                Amount = x.OrderedAmount
            }).OrderByDescending(x => x.CreatedAt)
            .Take(_recentOrderCount)
            .ToListAsync(cancellationToken);

        return orders;
    }
}
