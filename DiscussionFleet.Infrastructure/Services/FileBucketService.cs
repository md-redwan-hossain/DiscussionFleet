using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using DiscussionFleet.Application.Common.Options;
using DiscussionFleet.Application.Common.Services;
using DiscussionFleet.Application.MembershipFeatures.DataTransferObjects;
using DiscussionFleet.Domain.Entities.Enums;
using Microsoft.Extensions.Options;

namespace DiscussionFleet.Infrastructure.Services;

public class FileBucketService : IFileBucketService
{
    private readonly IAmazonS3 _s3Client;
    private readonly FileBucketOptions _fileBucketOptions;

    public FileBucketService(IAmazonS3 s3Client, IOptions<FileBucketOptions> options)
    {
        _s3Client = s3Client;
        _fileBucketOptions = options.Value;
    }


    public async Task<bool> DoesBucketExistAsync(string bucketName)
    {
        return await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
    }

    public async Task CreateBucketAsync(string bucketName)
    {
        var putBucketRequest = new PutBucketRequest
        {
            BucketName = bucketName,
            UseClientRegion = true
        };

        await _s3Client.PutBucketAsync(putBucketRequest);
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

    public async Task<bool> UploadImageAsync(ImageUploadRequest dto)
    {
        if (await DoesBucketExistAsync(_fileBucketOptions.BucketName) is false)
        {
            await CreateBucketAsync(_fileBucketOptions.BucketName);
        }

        var putObjectRequest = new PutObjectRequest
        {
            BucketName = _fileBucketOptions.BucketName,
            Key = $"{dto.Purpose.ToString()}_{dto.Id}{dto.FileExtension}",
            ContentType = dto.ContentType,
            InputStream = dto.ReadStream
        };
        var result = await _s3Client.PutObjectAsync(putObjectRequest);
        return result.HttpStatusCode is HttpStatusCode.OK;
    }


    public async Task<bool> DeleteImageAsync(Guid id, ImagePurpose purpose, string fileExtension)
    {
        if (await DoesBucketExistAsync(_fileBucketOptions.BucketName) is false)
        {
            await CreateBucketAsync(_fileBucketOptions.BucketName);
        }

        var deleteObjectRequest = new DeleteObjectRequest
        {
            BucketName = _fileBucketOptions.BucketName,
            Key = $"{purpose.ToString()}_{id}{fileExtension}",
        };

        var response = await _s3Client.DeleteObjectAsync(deleteObjectRequest);
        return response.HttpStatusCode is HttpStatusCode.OK;
    }
}