namespace DiscussionFleet.Application.QuestionFeatures.DataTransferObjects;

public record RelatedQuestionResponse(Guid Id, string Title, int VoteCount, string Url, DateTime LastActivity);