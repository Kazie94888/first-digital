using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Application.Enums;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.Wallets;

internal sealed class WalletsFormQueryHandler : IQueryHandler<WalletsFormQuery, ApplicationFormDto>
{
    private readonly ReadOnlyDataContext _context;

    public WalletsFormQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<ApplicationFormDto>> Handle(WalletsFormQuery request, CancellationToken cancellationToken)
    {
        var application = await _context.Applications.FirstAsync(x => x.Id == request.ApplicationId, cancellationToken);

        var walletForm = new ApplicationFormDto(ApplicationFormType.Wallets);

        var allWallets = application.Wallets?.Wallets ?? [];
        foreach (var item in allWallets)
        {
            walletForm.AddItem(new VerifiableSectionDto<WalletsRequest>
            {
                Id = item.Id.Value,
                Verified = item.IsVerified(),
                Data = new WalletsRequest
                {
                    Address = item.Address,
                    Network = item.Network,
                    CreatedAt = item.CreatedAt
                }
            });
        }

        return Result.Ok(walletForm);
    }
}