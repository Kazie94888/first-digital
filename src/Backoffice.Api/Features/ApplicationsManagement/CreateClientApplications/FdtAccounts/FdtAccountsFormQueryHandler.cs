using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Application.Enums;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.FdtAccounts;

internal sealed class FdtAccountsFormQueryHandler : IQueryHandler<FdtAccountsFormQuery, ApplicationFormDto>
{
    private readonly ReadOnlyDataContext _context;

    public FdtAccountsFormQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<ApplicationFormDto>> Handle(FdtAccountsFormQuery request, CancellationToken cancellationToken)
    {
        var application = await _context.Applications.FirstAsync(x => x.Id == request.ApplicationId, cancellationToken);

        var fdtAccountForm = new ApplicationFormDto(ApplicationFormType.FdtAccounts);

        var allFdtAccounts = application.FdtAccounts?.FdtAccounts ?? [];
        foreach (var item in allFdtAccounts)
        {
            fdtAccountForm.AddItem(new VerifiableSectionDto<FdtAccountRequest>
            {
                Id = item.Id.Value,
                Verified = item.IsVerified(),
                Data = new FdtAccountRequest
                {
                    ClientName = item.ClientName,
                    AccountNumber = item.AccountNumber,
                    CreatedAt = item.CreatedAt
                }
            });
        }

        return Result.Ok(fdtAccountForm);
    }
}