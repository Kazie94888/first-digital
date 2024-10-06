using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;

namespace SmartCoinOS.Backoffice.Api.Features.Settings.DepositBanks;

internal sealed class DepositBankDetailsQueryHandler : IQueryHandler<DepositBankDetailsQuery, DepositBankDetails>
{
    private readonly ReadOnlyDataContext _context;

    public DepositBankDetailsQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<DepositBankDetails>> Handle(DepositBankDetailsQuery request, CancellationToken cancellationToken)
    {
        var ddb = await _context.DepositBanks.Include(x => x.Address)
                                            .FirstAsync(x => x.Id == request.Id, cancellationToken);

        var depositBank = new DepositBankDetails
        {
            Id = ddb.Id,
            Name = ddb.Name,
            Beneficiary = ddb.Beneficiary,
            CreatedAt = ddb.CreatedAt,
            Iban = ddb.Iban,
            Swift = ddb.Swift,
            Archived = ddb.Archived,
            Address = ddb.Address.GetFullAddress(),
            Default = ddb.Default
        };

        return Result.Ok(depositBank);
    }
}