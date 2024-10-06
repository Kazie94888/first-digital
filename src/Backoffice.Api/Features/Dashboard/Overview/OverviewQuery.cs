using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;

namespace SmartCoinOS.Backoffice.Api.Features.Dashboard.Overview;

internal sealed record OverviewQuery : IQuery<OverviewResponse>
{
    [FromQuery(Name = "timeframe")]
    public string? Timeframe { get; init; }

    public DateTimeOffset? GetFromDate()
    {
        DateTimeOffset? startDate = null;

        if (!string.IsNullOrEmpty(Timeframe))
        {
            var today = DateTimeOffset.UtcNow;

            startDate = Timeframe switch
            {
                "7d" => today.AddDays(-7),
                "30d" => today.AddDays(-30),
                "90d" => today.AddDays(-90),
                "180d" => today.AddDays(-180),
                "1y" => today.AddYears(-1),
                _ => null
            };
        }

        return startDate;
    }
}
