using DiscussionFleet.Domain.Entities.MemberAggregate;
using DiscussionFleet.Domain.Entities.TagAggregate;

namespace DiscussionFleet.Application.QuestionFeatures.DataTransferObjects;

public record QuestionCreateRequest(MemberId AuthorId, string Title, string Body, ICollection<TagId> ExistingTags);

