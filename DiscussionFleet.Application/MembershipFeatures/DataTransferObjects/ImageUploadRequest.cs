using DiscussionFleet.Domain.Entities.MemberAggregate;
using DiscussionFleet.Domain.Entities.MultimediaImageAggregate;

namespace DiscussionFleet.Application.MembershipFeatures.DataTransferObjects;

public record ImageUploadRequest(
    Stream ReadStream,
    string ContentType,
    string FileExtension,
    MemberId Id,
    ImagePurpose Purpose);