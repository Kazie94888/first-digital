using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.Wallets;

public sealed record SetWalletsFormCommand : ICommand<ApplicationFormMetadataDto>
{
    [FromRoute(Name = "applicationId")]
    public required ApplicationId ApplicationId { get; init; }

    [FromBody]
    public required List<WalletsRequest> Wallets { get; init; } = [];
}

public sealed record WalletsRequest
{
    public required string Address { get; init; }
    public required BlockchainNetwork Network { get; init; }

    public DateTimeOffset? CreatedAt { get; init; }
}
