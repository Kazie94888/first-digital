using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using SmartCoinOS.Infrastructure.Settings.Interfaces;

namespace SmartCoinOS.Infrastructure.Settings;

public sealed class ConfigurationChangedNotifier : IConfigurationChangedNotifier
{
    private ConfigurationReloadToken? _changeToken;
    public void NotifyChange()
    {
        _changeToken?.OnReload();
    }

    public IChangeToken GetReloadToken()
    {
        _changeToken = new ConfigurationReloadToken();
        return _changeToken;
    }
}