using System.Text.Json.Serialization;

namespace SmartCoinOS.Infrastructure.Options;

public sealed class TracingOptions
{
    /// <summary>
    /// Endpoint for trace collector
    /// </summary>
    public string? CollectorUrl { get; set; }

    /// <summary>
    /// Specifies the exporter to use
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TracingCollector CollectorName { get; set; }

    public bool EnableDatabaseTracing { get; set; } = false;
}

public enum TracingCollector
{
    Zipkin,
    Tempo
}