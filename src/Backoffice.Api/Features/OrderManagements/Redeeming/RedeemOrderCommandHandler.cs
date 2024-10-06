﻿using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Deposit;
using SmartCoinOS.Domain.Orders;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Redeeming;

internal sealed class RedeemOrderCommandHandler : ICommandHandler<RedeemOrderCommand, EntityId>
{
    private readonly DataContext _context;
    private readonly OrderService _orderService;

    public RedeemOrderCommandHandler(DataContext context, OrderService orderService)
    {
        _context = context;
        _orderService = orderService;
    }

    public async Task<Result<EntityId>> Handle(RedeemOrderCommand command, CancellationToken cancellationToken)
    {
        var redeemRequest = command.Request;
        var clientId = redeemRequest.ClientId;

        var depositBank = await GetDefaultDepositBankAsync(clientId);
        var clientStatus = await _context.Clients.Where(x => x.Id == clientId).Select(x => x.Status)
            .FirstAsync(cancellationToken);
        var depositWallet = await GetDefaultDepositWalletAsync(cancellationToken);
        var depositFdtAccount = await _context.FdtDepositAccounts.Select(x => x.Id).FirstAsync(cancellationToken);

        var clientData = new ClientOrderData()
        {
            ClientId = redeemRequest.ClientId,
            WalletId = redeemRequest.WalletId,
            BankAccountId = redeemRequest.BankAccountId,
            FdtAccountId = redeemRequest.FdtAccountId,
            ClientStatus = clientStatus
        };

        var fdtData = new FdtOrderData()
        {
            FdtAccountId = depositFdtAccount,
            WalletId = depositWallet,
            BankId = depositBank
        };

        var order = await _orderService.CreateRedeemOrderAsync(clientData,
            fdtData,
            redeemRequest.RedeemAmount,
            command.UserInfo,
            cancellationToken);

        _context.RedeemOrders.Add(order);

        return Result.Ok(new EntityId(order.Id.Value));
    }

    private async Task<DepositBankId> GetDefaultDepositBankAsync(ClientId clientId)
    {
        var clientDefaultDeposit = await _context.Clients.Where(x => x.Id == clientId)
                                       .Select(x => x.DepositBankId).FirstOrDefaultAsync()
                                   ?? await _context.DepositBanks.Where(x => x.Default)
                                       .Select(x => x.Id).FirstAsync();

        return clientDefaultDeposit;
    }

    private async Task<DepositWalletId> GetDefaultDepositWalletAsync(CancellationToken cancellationToken)
    {
        var depositWallets = await _context.DepositWallets.ToListAsync(cancellationToken);

        var defaultDeposit = depositWallets.Find(x => x.Default);
        
        return defaultDeposit?.Id ?? depositWallets[0].Id;
    }
}