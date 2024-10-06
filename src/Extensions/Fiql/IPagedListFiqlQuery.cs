namespace SmartCoinOS.Extensions.Fiql;

public interface IPagedListFiqlQuery : IPagedListQuery
{
    public string? FiqlQuery { get; init; }
}
