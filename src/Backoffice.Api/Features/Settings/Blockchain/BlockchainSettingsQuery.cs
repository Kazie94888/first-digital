using SmartCoinOS.Application.Abstractions.Messaging;

namespace SmartCoinOS.Backoffice.Api.Features.Settings.Blockchain;

public sealed record BlockchainSettingsQuery : IQuery<BlockchainSettingsResponse>;

public sealed record BlockchainSettingsResponse
{
    public required string SafeAddress { get; init; }
    public required string SmartCoinAddress { get; init; }
    public required Dictionary<string, string> Signers { get; init; }
}
