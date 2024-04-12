using DiscussionFleet.Contracts.Membership;
using DiscussionFleet.Domain.Entities.Enums;

namespace DiscussionFleet.Application.Common.Services;

public interface IFileBucketService
{
    Task<bool> UploadImageAsync(ImageUploadRequest dto);
    Task<string> GetImageUrlAsync(string key, uint ttlInMinute = 60);
    Task<string> GetImageUrlAsync(Guid id, ImagePurpose purpose, string fileExtension, uint ttlInMinute = 60);
    Task<bool> DeleteImageAsync(Guid id, ImagePurpose purpose, string fileExtension);
}