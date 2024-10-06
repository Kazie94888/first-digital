using SmartCoinOS.Application.Abstractions.Messaging;

namespace SmartCoinOS.Backoffice.Api.Features.FileStorage;

public sealed record DocumentTypeListQuery : IQuery<Dictionary<string, IReadOnlyList<string>>>;