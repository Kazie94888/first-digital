namespace AWSServiceProvider.Services.S3;

public interface IFileService
{
    Task<bool> UploadAsync(UploadFileDto file, CancellationToken cancellationToken);
    Task<bool> UploadAsync(IEnumerable<UploadFileDto> dtos, CancellationToken cancellationToken);
    Task<byte[]> DownloadAsync(string key, CancellationToken cancellationToken);
    Task DeleteAsync(string key, CancellationToken cancellationToken);
}