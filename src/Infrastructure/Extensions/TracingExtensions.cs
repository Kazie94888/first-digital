using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SmartCoinOS.Infrastructure.Options;

namespace SmartCoinOS.Infrastructure.Extensions;

public static class TracingExtensions
{
    public static IServiceCollection AddTracing(this IServiceCollection services, Action<TracingOptions> configure,
        string serviceName)
    {
        var options = new TracingOptions();
        configure.Invoke(options);

        if (string.IsNullOrWhiteSpace(options.CollectorUrl) ||
            !Uri.TryCreate(options.CollectorUrl, UriKind.Absolute, out var collectorUrl))
            throw new InvalidOperationException(nameof(options.CollectorUrl));

        services.AddOpenTelemetry()
            .ConfigureResource(opts =>
            {
                opts.AddService(serviceName: serviceName);
            })
            .WithTracing(builder =>
            {
                builder.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();
                if (options.EnableDatabaseTracing)
                    builder.AddEntityFrameworkCoreInstrumentation(opts =>
                    {
                        opts.EnrichWithIDbCommand = (activity, command) =>
                        {
                            activity.SetTag("db.query", command.CommandText);
                        };
                    });
                switch (options.CollectorName)
                {
                    case TracingCollector.Zipkin:
                        builder.AddZipkinExporter(opts => opts.Endpoint = collectorUrl);
                        break;
                    case TracingCollector.Tempo:
                        builder.AddOtlpExporter(opts =>
                        {
                            opts.Endpoint = collectorUrl;
                            opts.Protocol = OtlpExportProtocol.Grpc;
                        });
                        break;
                    default:
                        throw new InvalidEnumArgumentException("Invalid exporter name value",
                            (int)options.CollectorName, typeof(TracingCollector));
                }
            });
        return services;
    }
}
