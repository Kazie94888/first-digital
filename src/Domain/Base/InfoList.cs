namespace SmartCoinOS.Domain.Base;

public sealed class InfoList<T> : BaseInfoList<T> where T : class
{
    public InfoList(IReadOnlyList<T> items)
    {
        Data = items;
    }
}
