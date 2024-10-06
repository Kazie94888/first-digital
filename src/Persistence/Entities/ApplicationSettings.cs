namespace SmartCoinOS.Persistence.Entities;
public sealed class ApplicationSettings
{
    public ApplicationSettings(string key, string value, string displayName)
    {
        Key = key;
        Value = value;
        DisplayName = displayName;
    }

    public string Key { get; }
    public string? Value { get; private set; }
    public string DisplayName { get; private set; }

    public void SetValue(string? value)
    {
        Value = value;
    }

    public void SetDisplayName(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentNullException(nameof(displayName));

        DisplayName = displayName;
    }
}