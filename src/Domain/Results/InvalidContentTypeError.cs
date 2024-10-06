namespace SmartCoinOS.Domain.Results;

public sealed class InvalidContentTypeError : Error
{
    public InvalidContentTypeError(string contentType, IEnumerable<string> acceptableContentTypes)
        : base($"The content type '{contentType}' is not acceptable. Acceptable content types are: {string.Join(", ", acceptableContentTypes)}")
    {
        Reasons.Add(new Error(Message));
    }
}