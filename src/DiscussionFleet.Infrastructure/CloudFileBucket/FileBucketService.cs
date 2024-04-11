using System.ComponentModel;
using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using DiscussionFleet.Application.Common.Options;
using DiscussionFleet.Application.Common.Services;
using DiscussionFleet.Domain.Entities.Enums;
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

    public async Task<string> GetImageUrlAsync(string key, uint ttlInMinute = 60)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _fileBucketOptions.BucketName,
            Key = key,
            Expires = DateTime.UtcNow.AddHours(ttlInMinute)
        };
        var url = await _s3Client.GetPreSignedURLAsync(request);
        return url;
    }


    public async Task<string> GetImageUrlAsync(Guid id, ImagePurpose purpose, string fileExtension,
        uint ttlInMinute = 60)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _fileBucketOptions.BucketName,
            Key = $"{purpose.ToString()}_{id}{fileExtension}",
            Expires = DateTime.UtcNow.AddHours(ttlInMinute)
        };
        var url = await _s3Client.GetPreSignedURLAsync(request);
        return url;
    }

    public async Task<bool> UploadImageAsync(Stream readStream, string contentType, string fileExtension, Guid id,
        ImagePurpose purpose)
    {
        var putObjectRequest = new PutObjectRequest
        {
            BucketName = _fileBucketOptions.BucketName,
            Key = $"{purpose.ToString()}_{id}{fileExtension}",
            ContentType = contentType,
            InputStream = readStream
        };
        var result = await _s3Client.PutObjectAsync(putObjectRequest);
        return result.HttpStatusCode is HttpStatusCode.OK;
    }
}