﻿using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.FdtAccounts;

internal sealed class VerifyFdtAccountCommandHandler : ICommandHandler<VerifyFdtAccountCommand, EntityId>
{
    private readonly DataContext _context;

    public VerifyFdtAccountCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<EntityId>> Handle(VerifyFdtAccountCommand request, CancellationToken cancellationToken)
    {
        var client = await _context.Clients.FirstAsync(x => x.Id == request.ClientId, cancellationToken);

        var verifyResult = client.VerifyFdtAccount(request.FdtAccountId, request.UserInfo);
        if (verifyResult.IsFailed)
            return Result.Fail(verifyResult.Errors);

        return Result.Ok(new EntityId(request.FdtAccountId.Value));
    }
}