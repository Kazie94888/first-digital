using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AWSServiceProvider.Services.S3;

public class FileService : IFileService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private readonly ILogger<FileService> _logger;

    public FileService(IAmazonS3 s3Client, IOptions<AwsS3Configuration> config, ILogger<FileService> logger)
    {
        _s3Client = s3Client;
        _logger = logger;
        _bucketName = config.Value.BucketName;
    }

    private async Task<bool> DoUploadAsync(UploadFileDto dto, CancellationToken cancellationToken)
    {
        try
        {
            using var stream = new MemoryStream(dto.ByteArray);
            stream.Position = 0;

            await _s3Client.UploadObjectFromStreamAsync(_bucketName, dto.Key, stream, null, cancellationToken);

            return true;
        }
        catch (AmazonS3Exception s3Exception)
        {
            _logger.LogError(s3Exception,
                "Failed to upload a file ({FileName}) with error: {ErrorCode} | {ErrorMessage}",
                dto.FileName, s3Exception.ErrorCode, s3Exception.Message);

            return false;
        }
    }

    public async Task<bool> UploadAsync(UploadFileDto file, CancellationToken cancellationToken)
    {
        return await DoUploadAsync(file, cancellationToken);
    }

    public async Task<bool> UploadAsync(IEnumerable<UploadFileDto> dtos, CancellationToken cancellationToken)
    {
        var isAllUploaded = true;
        var uploadedKeys = new List<string>();
        foreach (var uploadFileDto in dtos)
        {
            var isUploaded = await DoUploadAsync(uploadFileDto, cancellationToken);
            if (isUploaded)
                uploadedKeys.Add(uploadFileDto.Key);
            else
            {
                isAllUploaded = false;
                break;
            }
        }

        if (isAllUploaded)
            return true;

        foreach (var key in uploadedKeys)
        {
            await DeleteAsync(key, cancellationToken);
        }

        return false;
    }

    public async Task<byte[]> DownloadAsync(string key, CancellationToken cancellationToken)
    {
        try
        {
            var downloadRequest = new GetObjectRequest
            {
                BucketName = _bucketName,
                Key = key
            };
            using var response = await _s3Client.GetObjectAsync(downloadRequest, cancellationToken);
            using var ms = new MemoryStream();
            await response.ResponseStream.CopyToAsync(ms, cancellationToken);

            return ms.ToArray();
        }
        catch (AmazonS3Exception ex) when (ex.ErrorCode == "NoSuchKey")
        {
            return [];
        }
    }

    public async Task DeleteAsync(string key, CancellationToken cancellationToken)
    {
        var deleteRequest = new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = key
        };

        await _s3Client.DeleteObjectAsync(deleteRequest, cancellationToken);
    }
}