using Spectre.Console.Cli;

namespace SmartCoinOS.SmartCoinTool.Common;

internal sealed class SettingsWithCancellation : CommandSettings
{
    public CancellationToken CancellationToken { get; set; } = CancellationToken.None;
}