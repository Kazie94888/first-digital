using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Extensions.Fiql;

namespace SmartCoinOS.Backoffice.Api.Application;

public abstract record PagedListFiqlQuery : PagedListQuery, IPagedListFiqlQuery
{
    [FromQuery(Name = "_filter")]
    public string? FiqlQuery { get; init; }
}
