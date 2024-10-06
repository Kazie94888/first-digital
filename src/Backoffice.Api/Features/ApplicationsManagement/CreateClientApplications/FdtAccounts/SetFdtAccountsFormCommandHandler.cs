using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.FdtAccounts;

internal sealed class SetFdtAccountsFormCommandHandler : ICommandHandler<SetFdtAccountsFormCommand, ApplicationFormMetadataDto>
{
    private readonly DataContext _context;

    public SetFdtAccountsFormCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<ApplicationFormMetadataDto>> Handle(SetFdtAccountsFormCommand request, CancellationToken cancellationToken)
    {
        var application = await _context.Applications.FirstAsync(x => x.Id == request.ApplicationId, cancellationToken);

        application.ClearFdtAccounts();

        foreach (var account in request.FdtAccounts)
        {
            var addResult = application.SetFdtAccounts(account.ClientName, account.AccountNumber);

            if (addResult.IsFailed)
                return addResult;
        }

        var response = new ApplicationFormMetadataDto
        {
            PrettyId = application.ApplicationNumber,
            Id = application.Id,
        };

        return Result.Ok(response);
    }
}