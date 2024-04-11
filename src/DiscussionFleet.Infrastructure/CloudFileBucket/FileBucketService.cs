using System.ComponentModel;
using Amazon.S3;
using Amazon.S3.Model;
using DiscussionFleet.Application.Common.Enums;
using DiscussionFleet.Application.Common.Options;
using DiscussionFleet.Application.Common.Services;
using Microsoft.Extensions.Options;

namespace DiscussionFleet.Infrastructure.CloudFileBucket;

public class FileBucketService : IFileBucketService
{
    private readonly IAmazonS3 _s3Client;
    private readonly FileBucketOptions _fileBucketOptions;


    public FileBucketService(IAmazonS3 s3Client, IOptions<FileBucketOptions> options)
    {
        _s3Client = s3Client;
        _fileBucketOptions = options.Value;
    }


    public async Task<string> GetImageUrlAsync(Guid id, ImagePurpose purpose, uint ttlInMinute)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _fileBucketOptions.BucketName,
            Key = $"{nameof(purpose)}_{id}",
            Expires = DateTime.UtcNow.AddHours(ttlInMinute)
        };
        var url = await _s3Client.GetPreSignedURLAsync(request);
        
        return url;
    }

    public async Task UploadImageAsync(Stream readStream, Guid id, string contentType, ImagePurpose purpose)
    {
        var putObjectRequest = new PutObjectRequest
        {
            BucketName = _fileBucketOptions.BucketName,
            Key = $"{nameof(purpose)}_{id}",
            ContentType = contentType,
            InputStream = readStream
        };
        var result = await _s3Client.PutObjectAsync(putObjectRequest);
    }
}