using SmartCoinOS.Domain.Application.Events;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.SeedWork;
using StronglyTypedIds;

namespace SmartCoinOS.Domain.Shared;

public sealed class Document : AggregateRoot
{
    public const long MaxSizeInBytes = 10 * 1024 * 1024; // 10 MB

    public static readonly List<string> AcceptableContentTypes =
    [
        "application/pdf",
        "application/vnd.ms-excel",
        "application/msword",
        "text/csv",
        "image/jpeg",
        "image/jpg",
        "image/png"
    ];

    private Document()
    {
    }

    public DocumentId Id { get; init; }
    public required string FileName { get; init; }
    public required string Key { get; init; }
    public required string ContentType { get; init; }
    public required long SizeInBytes { get; init; }
    public bool Archived { get; private set; }
    public DateTimeOffset? ArchivedAt { get; private set; }

    public ApplicationId? ApplicationId { get; private set; }
    public string? ApplicationDocumentType { get; private set; }

    public ClientId? ClientId { get; private set; }
    public string? ClientDocumentType { get; private set; }

    public static Document CreateTempDocument(
        DocumentId id,
        string key,
        string fileName,
        string contentType,
        long sizeInBytes,
        ClientId? clientId,
        UserInfo createdBy
        )
    {
        var document = new Document
        {
            Id = id,
            Key = key,
            FileName = fileName,
            ContentType = contentType,
            SizeInBytes = sizeInBytes,
            CreatedBy = createdBy,
            ClientId = clientId
        };

        return document;
    }

    public void ConnectApplication(ApplicationId applicationId, string applicationDocumentType)
    {
        ApplicationId = applicationId;
        ApplicationDocumentType = applicationDocumentType;
    }

    public void ConnectClient(ClientId clientId, string clientDocumentType)
    {
        ClientId = clientId;
        ClientDocumentType = clientDocumentType;
    }

    public static Result Validate(string contentType, long sizeInBytes)
    {
        if (!AcceptableContentTypes.Contains(contentType.ToLower()))
            return Result.Fail(new InvalidContentTypeError(contentType, AcceptableContentTypes));

        if (sizeInBytes > MaxSizeInBytes)
            return Result.Fail(new InvalidSizeInBytesError(sizeInBytes, MaxSizeInBytes));

        return Result.Ok();
    }

    public Result Archive()
    {
        if (Archived)
            return new EntityAlreadyArchivedError(nameof(Document), "Document is already archived.");

        Archived = true;
        ArchivedAt = DateTimeOffset.UtcNow;

        if (ApplicationId.HasValue)
            AddDomainEvent(new ApplicationDocumentArchivedEvent(Id, Application.ApplicationId.Parse(ApplicationId.Value.ToString())));

        return Result.Ok();
    }
}

[StronglyTypedId]
public readonly partial struct DocumentId;