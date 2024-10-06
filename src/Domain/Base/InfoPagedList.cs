namespace SmartCoinOS.Domain.Base;

public sealed class InfoPagedList<T> : BaseInfoList<T> where T : class
{
    public InfoPagedList(IReadOnlyList<T> items, int totalCount, int currentPage, int pageSize)
    {
        Data = items;

        Info.Add("page", currentPage);
        Info.Add("pageSize", pageSize);
        Info.Add("total", totalCount);
    }
}
