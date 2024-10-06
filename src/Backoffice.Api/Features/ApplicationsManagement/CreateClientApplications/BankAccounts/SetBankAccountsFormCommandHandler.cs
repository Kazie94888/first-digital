using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Application;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.BankAccounts;

internal sealed class
    SetBankAccountsFormCommandHandler : ICommandHandler<SetBankAccountsFormCommand, ApplicationFormMetadataDto>
{
    private readonly DataContext _context;

    public SetBankAccountsFormCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<ApplicationFormMetadataDto>> Handle(SetBankAccountsFormCommand command,
        CancellationToken cancellationToken)
    {
        var application = await _context.Applications.FirstAsync(x => x.Id == command.ApplicationId, cancellationToken);

        application.ClearBankAccounts();

        foreach (var conto in command.BankAccounts)
        {
            if (conto.Documents.Count == 0)
                return Result.Fail("You should provide at least one document");

            var smartTrustBank = new SmartTrustBank(conto.SmartTrustOwnBankId, conto.SmartTrustThirdPartyBankId);

            var address = new AddressRecord()
            {
                Country = conto.Country,
                City = conto.City,
                State = conto.State,
                PostalCode = conto.PostalCode,
                Street = conto.Street
            };

            var documents = conto.Documents.Select(x => (ApplicationDocument)x).ToList();

            var setResult = application.SetBankAccounts(conto.Beneficiary, conto.BankName, conto.Iban, conto.Swift,
                smartTrustBank,
                address, documents, command.UserInfo);

            if (setResult.IsFailed)
                return setResult;
        }

        var response = new ApplicationFormMetadataDto
        {
            PrettyId = application.ApplicationNumber, Id = application.Id,
        };

        return Result.Ok(response);
    }
}
