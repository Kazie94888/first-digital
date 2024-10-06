namespace SmartCoinOS.Extensions.Fiql;

public interface IPagedListQuery
{
    public string? SortQuery { get; init; }
    public int Page { get; init; }
    public int Take { get; init; }

    public int Skip { get; }
}
