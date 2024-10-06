using SmartCoinOS.Domain.Shared;

namespace SmartCoinOS.Domain.Application;

public sealed record ApplicationDocument
{
    public required string DocumentType { get; init; }
    public required DocumentId DocumentId { get; init; }
    public string? FileName { get; init; }
}
