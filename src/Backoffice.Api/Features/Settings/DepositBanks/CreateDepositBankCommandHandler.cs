using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Deposit;

namespace SmartCoinOS.Backoffice.Api.Features.Settings.DepositBanks;

internal sealed class CreateDepositBankCommandHandler : ICommandHandler<CreateDepositBankCommand, EntityId>
{
    private readonly DataContext _context;

    public CreateDepositBankCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<EntityId>> Handle(CreateDepositBankCommand command, CancellationToken cancellationToken)
    {
        var account = command.Request.DepositBankAccount;
        var address = command.Request.DepositBankAddress;
        var createdBy = command.UserInfo;

        var nonArchivedBanksCount = await _context.DepositBanks.CountAsync(x => !x.Archived, cancellationToken);
        var isDefault = nonArchivedBanksCount == 0;

        var addressRecord = new AddressRecord()
        {
            Country = address.Country,
            State = address.State,
            City = address.City,
            PostalCode = address.PostalCode,
            Street = address.Street
        };

        var depositBank = DepositBank.Create(account.BankName, account.Beneficiary, account.SwiftCode, account.Iban,
            addressRecord, isDefault, createdBy);

        _context.DepositBanks.Add(depositBank);

        return Result.Ok(new EntityId(depositBank.Id.Value));
    }
}
