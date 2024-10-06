using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Orders;
using SmartCoinOS.Integrations.FdtPartnerApi.Contracts;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Minting;

internal sealed class SetDepositInstructionCompletedCommandHandler : ICommandHandler<SetDepositInstructionCompletedCommand, EntityId>
{
    private readonly DataContext _context;
    private readonly IFdtPartnerApiClient _fdtClient;
    public SetDepositInstructionCompletedCommandHandler(DataContext context, IFdtPartnerApiClient fdtClient)
    {
        _context = context;
        _fdtClient = fdtClient;
    }

    public async Task<Result<EntityId>> Handle(SetDepositInstructionCompletedCommand command, CancellationToken cancellationToken)
    {
        var order = await (from o in _context.MintOrders
                           where o.DepositInstruction != null
                                    && o.DepositInstruction.Id == command.DepositInstructionId
                                    && o.DepositInstruction.ReferenceNumber == command.ReferenceNumber
                           select o).FirstOrDefaultAsync(cancellationToken);

        if (order is null)
            return Result.Fail($"Order '{command.ReferenceNumber}' couldn't be found.");

        var transactions = await GetDepositTransactionsAsync(command.DepositInstructionId, cancellationToken);

        var depositInstrResult = order.CompleteDepositInstruction(transactions, command.UserInfo);
        if (depositInstrResult.IsFailed)
            return depositInstrResult;

        return Result.Ok(new EntityId(order.Id.Value));
    }

    private async Task<IReadOnlyList<Money>> GetDepositTransactionsAsync(int depositInstructionId, CancellationToken cancellationToken)
    {
        var transactions = await _fdtClient.GetDepositTransactionsAsync(depositInstructionId, cancellationToken);

        IReadOnlyList<Money> moneyTransactions = (from t in transactions
                                                  select new Money
                                                  {
                                                      Amount = t.Amount,
                                                      Currency = t.AssetSymbol
                                                  }).ToList();

        return moneyTransactions;
    }
}
