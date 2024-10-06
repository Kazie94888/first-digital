namespace SmartCoinOS.Backoffice.Api.Base.Options;

internal sealed record BlockchainSettings
{
    public required string SafeAddress { get; init; }
    public required string SmartCoinAddress { get; init; }
    public required Dictionary<string, string> Signers { get; init; }
}