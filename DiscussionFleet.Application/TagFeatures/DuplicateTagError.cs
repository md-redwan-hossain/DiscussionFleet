namespace DiscussionFleet.Application.TagFeatures;


public record DuplicateTagError(ICollection<string> DuplicateData);