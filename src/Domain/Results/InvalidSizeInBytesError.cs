namespace SmartCoinOS.Domain.Results;

public sealed class InvalidSizeInBytesError : Error
{
    public InvalidSizeInBytesError(long sizeInBytes, long maxSizeInBytes)
        : base($"The size in bytes '{sizeInBytes:N}' exceeds the maximum size of '{maxSizeInBytes:N}'")
    {
        Reasons.Add(new Error(Message));
    }
}