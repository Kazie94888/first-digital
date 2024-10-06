using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Minting;

public sealed record SetDepositInstructionCompletedCommand : ICommand<EntityId>
{
    public required int DepositInstructionId { get; init; }
    public required string ReferenceNumber { get; init; }

    public required UserInfoDto UserInfo { get; init; }
}
