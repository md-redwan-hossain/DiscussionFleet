using DiscussionFleet.Application.TagFeatures.DataTransferObjects;

namespace DiscussionFleet.Application.QuestionFeatures.DataTransferObjects;

public record QuestionWithNewTagsCreateRequest(
    Guid AuthorId,
    string Title,
    string Body,
    ICollection<Guid> ExistingTags,
    TagCreateRequest NewTagDto);