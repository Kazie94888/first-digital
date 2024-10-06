using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Redeeming;

public sealed record RedeemOrderPaymentCompletedCommand : ICommand<EntityId>
{
    public required int InstructionId { get; init; }
    public required string ReferenceNumber { get; init; }

    public required UserInfoDto UserInfo { get; init; }
}
