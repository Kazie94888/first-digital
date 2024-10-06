using Microsoft.Extensions.Primitives;

namespace SmartCoinOS.Infrastructure.Settings.Interfaces;

public interface IConfigurationChangedNotifier
{
    public void NotifyChange();
    public IChangeToken GetReloadToken();
}