using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.BankAccounts;

internal sealed class CreateBankAccountCommandHandler : ICommandHandler<CreateBankAccountCommand, EntityId>
{
    private readonly DataContext _context;

    public CreateBankAccountCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<EntityId>> Handle(CreateBankAccountCommand request, CancellationToken cancellationToken)
    {
        var client = await _context.Clients.FirstAsync(x => x.Id == request.ClientId, cancellationToken);

        var bank = request.BankAccount.BankAccountDetails;
        var address = request.BankAccount.BankAddress;

        var addressRecord = new AddressRecord()
        {
            Country = address.Country,
            State = address.State,
            City = address.City,
            PostalCode = address.PostalCode,
            Street = address.Street
        };
        
        var bankRecord = new CreatedClientBankAccountRecord
        {
            Beneficiary = bank.Beneficiary,
            Iban = bank.Iban,
            BankName = bank.BankName,
            SwiftCode = bank.SwiftCode,
            Alias = bank.Alias,
            SmartTrustBank = new SmartTrustBank(bank.SmartTrustOwnBankId, bank.SmartTrustThirdPartyBankId),
            Address = addressRecord
        };

        var result = client.AddBankAccount(bankRecord, request.UserInfo);

        if (result.IsFailed)
            return Result.Fail(result.Errors);

        var addedBank = result.Value;
        var verificationResult = client.VerifyBankAccount(addedBank.Id, request.UserInfo);
        if (result.IsFailed)
            return verificationResult;

        return Result.Ok(new EntityId(addedBank.Id.Value));
    }
}
