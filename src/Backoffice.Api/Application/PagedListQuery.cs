using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Extensions.Fiql;

namespace SmartCoinOS.Backoffice.Api.Application;
public abstract record PagedListQuery : IPagedListQuery
{
    [FromQuery(Name = "_sort")]
    public string? SortQuery { get; init; }

    [FromQuery(Name = "_page")]
    public int Page { get; init; }

    [FromQuery(Name = "_pageSize")]
    public int Take { get; init; }

    public int Skip => Page > 0 ? (Page - 1) * Take : 0;
}