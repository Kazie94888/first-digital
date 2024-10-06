using System.Text.Json;
using FluentResults;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Backoffice.Api.Features.OrderManagements.Minting;
using SmartCoinOS.Backoffice.Api.Features.OrderManagements.Redeeming;

namespace SmartCoinOS.Backoffice.Api.Features.Webhooks.SmartTrust;

internal static class SmartTrustFactory
{
    internal static Result<ICommand<EntityId>> GetCommand(string payload)
    {
        var jsonBody = JsonDocument.Parse(payload);
        if (!jsonBody.RootElement.TryGetProperty("detail-type", out var jsonElemType))
            return Result.Fail("Incompatible payload.");

        var eventType = jsonElemType.GetString();

        if (eventType == "FiatDepositInstructionDetails.Completed")
        {
            var detail = jsonBody.RootElement.GetProperty("detail");

            var instructionId = detail.GetProperty("Id").GetInt32();
            var referenceNumber = detail.GetProperty("ReferenceNumber").GetString();

            if (string.IsNullOrEmpty(referenceNumber))
                return Result.Fail("Required properties are missing.");

            var depositInstrCompleted = new SetDepositInstructionCompletedCommand
            {
                DepositInstructionId = instructionId,
                ReferenceNumber = referenceNumber,
                UserInfo = SmartTrustUser
            };

            return depositInstrCompleted;
        }
        else if (eventType == "FiatPaymentInstructionDetails.Completed")
        {
            var detail = jsonBody.RootElement.GetProperty("detail");
            var instructionId = detail.GetProperty("Id").GetInt32();
            var referenceNumber = detail.GetProperty("ReferenceNumber").GetString();
            if (string.IsNullOrEmpty(referenceNumber))
                return Result.Fail("Required properties are missing.");

            var paymentInstrCompleted = new RedeemOrderPaymentCompletedCommand
            {
                InstructionId = instructionId,
                ReferenceNumber = referenceNumber,
                UserInfo = SmartTrustUser
            };

            return paymentInstrCompleted;
        }

        return Result.Fail($"Event '{eventType}' is unknown.");
    }

    private static UserInfoDto SmartTrustUser => new()
    {
        Id = Guid.Empty,
        Type = Domain.Enums.UserInfoType.Application,
        Username = "SmartTrust"
    };
}
