using DiscussionFleet.Application.Common.Enums;

namespace DiscussionFleet.Application.Common.Services;

public interface IFileBucketService
{
    public Task UploadImageAsync(Stream readStream, Guid id, string contentType, ImagePurpose purpose);
}