using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Application.Enums;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.BankAccounts;

internal sealed class BankAccountsFormQueryHandler : IQueryHandler<BankAccountsFormQuery, ApplicationFormDto>
{
    private readonly ReadOnlyDataContext _context;

    public BankAccountsFormQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<ApplicationFormDto>> Handle(BankAccountsFormQuery request, CancellationToken cancellationToken)
    {
        var application = await _context.Applications.FirstAsync(x => x.Id == request.ApplicationId, cancellationToken);

        var bankForm = new ApplicationFormDto(ApplicationFormType.BankAccounts);

        var bankAccounts = application.BankAccounts?.BankRecords ?? [];
        foreach (var account in bankAccounts)
        {
            bankForm.AddItem(new VerifiableSectionDto<SetBankAccountsRequest>
            {
                Id = account.Id.Value,
                Verified = account.IsVerified(),
                Data = new SetBankAccountsRequest
                {
                    Beneficiary = account.Beneficiary,
                    BankName = account.BankName,
                    Iban = account.Iban,
                    Swift = account.Swift,
                    SmartTrustOwnBankId = account.SmartTrustOwnBankId,
                    SmartTrustThirdPartyBankId = account.SmartTrustThirdPartyBankId,
                    Street = account.Street,
                    City = account.City,
                    PostalCode = account.PostalCode,
                    State = account.State,
                    Country = account.Country,
                    CreatedAt = account.CreatedAt,

                    Documents = account.Documents.Select(x => (DocumentDto)x).ToList()
                }
            });
        }

        return Result.Ok(bankForm);
    }
}