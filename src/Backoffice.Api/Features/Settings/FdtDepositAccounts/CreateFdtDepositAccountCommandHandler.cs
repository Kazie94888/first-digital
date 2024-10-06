using FluentResults;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Deposit;

namespace SmartCoinOS.Backoffice.Api.Features.Settings.FdtDepositAccounts;

internal sealed class CreateFdtDepositAccountCommandHandler : ICommandHandler<CreateFdtDepositAccountCommand, EntityId>
{
    private readonly DataContext _dataContext;

    public CreateFdtDepositAccountCommandHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public Task<Result<EntityId>> Handle(CreateFdtDepositAccountCommand command, CancellationToken cancellationToken)
    {
        var accountNumber = new FdtDepositAccountNumber(command.FdtDepositAccount.AccountNumber);
        var fdtDepositAccount = DepositFdtAccount.Create(command.FdtDepositAccount.AccountName, accountNumber, command.UserInfo);

        _dataContext.FdtDepositAccounts.Add(fdtDepositAccount);

        return Task.FromResult(Result.Ok(new EntityId(fdtDepositAccount.Id.Value)));
    }
}