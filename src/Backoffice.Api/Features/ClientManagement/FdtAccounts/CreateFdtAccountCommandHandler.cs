using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.FdtAccounts;

internal sealed class CreateFdtAccountCommandHandler : ICommandHandler<CreateFdtAccountCommand, EntityId>
{
    private readonly DataContext _dataContext;

    public CreateFdtAccountCommandHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<Result<EntityId>> Handle(CreateFdtAccountCommand request, CancellationToken cancellationToken)
    {
        var client = await _dataContext.Clients.Where(x => x.Id == request.ClientId).FirstAsync(cancellationToken);

        var fdtAccount = request.Account;
        var addAccountResult = client.AddFdtAccount(fdtAccount.ClientName, fdtAccount.AccountNumber, fdtAccount.Alias, request.UserInfo);

        if (addAccountResult.IsFailed)
            return Result.Fail(addAccountResult.Errors);

        var addResult = addAccountResult.Value;
        var verificationResult = client.VerifyFdtAccount(addResult.Id, request.UserInfo);
        if (verificationResult.IsFailed)
            return verificationResult;

        return Result.Ok(new EntityId(addResult.Id.Value));
    }
}