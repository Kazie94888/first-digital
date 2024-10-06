using Amazon.S3;
using AWSServiceProvider.Services.S3;
using Microsoft.Extensions.DependencyInjection;

namespace AWSServiceProvider;

public static class AwsServiceSetup
{
    public static IServiceCollection AddAwsServiceProvider(this IServiceCollection services, Action<AwsS3Configuration> config)
    {
        services.Configure(config);
        services.AddAWSService<IAmazonS3>();

        services.AddTransient<IFileService, FileService>();

        return services;
    }
}