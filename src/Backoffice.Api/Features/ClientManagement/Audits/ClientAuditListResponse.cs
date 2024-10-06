using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.AuditLogs;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Audits;


public sealed record ClientAuditListResponse
{
    public required AuditLogId Id { get; init; }
    public required string Event { get; init; }
    public required string Description { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public required IEnumerable<AuditLogParameterDto> Parameters { get; init; }
    public required UserInfoDto InitiatedBy { get; init; }
}