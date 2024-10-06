using SmartCoinOS.Domain.AuditLogs;

namespace SmartCoinOS.Backoffice.Api.Base.Dto;

public sealed record AuditLogParameterDto
{
    public required string Name { get; init; }
    public required string Value { get; init; }

    public static implicit operator AuditLogParameterDto(AuditLogParameter parameter) =>
        new()
        {
            Name = parameter.Name,
            Value = parameter.Value
        };
}
