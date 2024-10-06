namespace SmartCoinOS.Domain.Base;

public abstract class BaseInfoList<T> where T : class
{
    public IReadOnlyList<T> Data { get; protected set; } = [];
    public Dictionary<string, object> Info { get; } = [];

    public void AddInfo(string key, object value)
    {
        Info.Add(key, value);
    }

    public void AddSort(string sortedBy)
    {
        var currentSort = (string)Info.GetValueOrDefault("sort", string.Empty);

        if (string.IsNullOrEmpty(currentSort))
            currentSort = sortedBy;
        else
            currentSort += $", {sortedBy}";

        Info["sort"] = currentSort;
    }
}
