using DiscussionFleet.Domain.Entities.Enums;

namespace DiscussionFleet.Application.Common.Services;

public interface IFileBucketService
{
    Task<bool> UploadImageAsync(Stream readStream, string contentType, string fileExtension, Guid id,
        ImagePurpose purpose);

    Task<string> GetImageUrlAsync(string key, uint ttlInMinute = 60);
    Task<string> GetImageUrlAsync(Guid id, ImagePurpose purpose, string fileExtension, uint ttlInMinute = 60);
}