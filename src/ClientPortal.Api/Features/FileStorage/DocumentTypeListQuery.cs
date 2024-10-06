using SmartCoinOS.Application.Abstractions.Messaging;

namespace SmartCoinOS.ClientPortal.Api.Features.FileStorage;

public sealed record DocumentTypeListQuery : IQuery<Dictionary<string, IReadOnlyList<string>>>;