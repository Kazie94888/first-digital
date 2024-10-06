using Spectre.Console.Cli;

namespace SmartCoinOS.SmartCoinTool.Common;
internal sealed class CancellationInterceptor : ICommandInterceptor, IDisposable
{
    private readonly CancellationTokenSource _tokenSource = new();

    public void Intercept(CommandContext context, CommandSettings settings)
    {
        if (settings is SettingsWithCancellation settingsWithCancellation)
            settingsWithCancellation.CancellationToken = _tokenSource.Token;
    }

    public void OnCancel(object? sender, ConsoleCancelEventArgs? args)
    {
        if (args?.Cancel ?? false)
            _tokenSource.Cancel();
    }

    public void Dispose() => _tokenSource?.Dispose();
}