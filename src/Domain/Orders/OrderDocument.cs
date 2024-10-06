using SmartCoinOS.Domain.SeedWork;
using StronglyTypedIds;

namespace SmartCoinOS.Domain.Orders;

public class OrderDocument : Entity
{
    internal OrderDocument() { }

    public required OrderDocumentId Id { get; init; }

    public required string Key { get; set; }
    public required string Name { get; set; }
    public required string DocumentType { get; set; }
    public required string ContentType { get; set; }

    public required OrderId OrderId { get; init; }
}

[StronglyTypedId]
public readonly partial struct OrderDocumentId;