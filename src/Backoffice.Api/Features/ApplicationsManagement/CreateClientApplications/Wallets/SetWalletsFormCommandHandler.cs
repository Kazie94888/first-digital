using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.Wallets;

internal sealed class SetWalletsFormCommandHandler : ICommandHandler<SetWalletsFormCommand, ApplicationFormMetadataDto>
{
    private readonly DataContext _context;

    public SetWalletsFormCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<ApplicationFormMetadataDto>> Handle(SetWalletsFormCommand request, CancellationToken cancellationToken)
    {
        var application = await _context.Applications.FirstAsync(x => x.Id == request.ApplicationId, cancellationToken);

        application.ClearWallets();

        foreach (var wallet in request.Wallets)
        {
            var addResult = application.SetWallets(wallet.Network, wallet.Address);

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