using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Formatting.Compact;

namespace SmartCoinOS.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static void AddLogging(this WebApplicationBuilder builder)
    {
        var useJsonLogger = builder.Configuration.GetValue("UseJsonLogger", false);

        var loggerConfig = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration);

        if (useJsonLogger)
            loggerConfig.WriteTo.Console(new CompactJsonFormatter());
        else
            loggerConfig.WriteTo.Console();

        Log.Logger = loggerConfig.CreateLogger();

        builder.Host.UseSerilog();
    }
}