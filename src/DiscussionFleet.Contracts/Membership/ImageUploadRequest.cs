using DiscussionFleet.Domain.Entities.Enums;

namespace DiscussionFleet.Contracts.Membership;

public record ImageUploadRequest(
    Stream ReadStream,
    string ContentType,
    string FileExtension,
    Guid Id,
    ImagePurpose Purpose);