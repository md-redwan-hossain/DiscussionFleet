namespace DiscussionFleet.Application.QuestionFeatures.DataTransferObjects;

public record QuestionCreateRequest(Guid AuthorId, string Title, string Body, ICollection<Guid> ExistingTags);