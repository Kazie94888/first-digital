namespace AWSServiceProvider.Services.S3;

public sealed record UploadFileDto
{
    public required string Key { get; set; }
    public required string FileName { get; set; }
    public required string ContentType { get; set; }
    public required long Length { get; set; }
    public required byte[] ByteArray { get; set; }
}