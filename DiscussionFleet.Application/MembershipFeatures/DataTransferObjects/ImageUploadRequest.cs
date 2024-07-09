using DiscussionFleet.Domain.Entities.Enums;

namespace DiscussionFleet.Application.MembershipFeatures.DataTransferObjects;

public record ImageUploadRequest(
    Stream ReadStream,
    string ContentType,
    string FileExtension,
    Guid Id,
    ImagePurpose Purpose);