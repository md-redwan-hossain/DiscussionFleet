namespace DiscussionFleet.Application.TagFeatures.DataTransferObjects;

public record TagCreateRequest(HashSet<string> TagTitles);