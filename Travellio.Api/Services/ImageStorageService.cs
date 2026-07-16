using Amazon.S3;
using Amazon.S3.Model;
using Travellio.Domain.DTOs;

namespace Travellio.Api.Services;

public class ImageStorageService(IAmazonS3 s3Client, IConfiguration configuration)
    : IImageStorageService
{
    public async Task<UploadedImage> UploadImageAsync(IFormFile file, CancellationToken cancellationToken)
    {
        var bucketName = configuration["R2:BucketName"];
        var publicUrl = configuration["R2:PublicUrl"];

        var extension = ImageContentTypes.Extensions[file.ContentType];
        var key = $"images/{Guid.CreateVersion7()}{extension}";

        await using var stream = file.OpenReadStream();
        var request = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            InputStream = stream,
            ContentType = file.ContentType,
            AutoCloseStream = false,
            // R2 doesn't support the SDK's SigV4 streaming/chunked payload signing.
            DisablePayloadSigning = true,
        };

        await s3Client.PutObjectAsync(request, cancellationToken);

        return new UploadedImage { Url = $"{publicUrl!.TrimEnd('/')}/{key}" };
    }
}

public static class ImageContentTypes
{
    public static readonly IReadOnlyDictionary<string, string> Extensions = new Dictionary<string, string>(
        StringComparer.OrdinalIgnoreCase)
    {
        ["image/jpeg"] = ".jpg",
        ["image/png"] = ".png",
        ["image/webp"] = ".webp",
        ["image/gif"] = ".gif",
    };

    public const long MaxFileSizeBytes = 10 * 1024 * 1024;
}