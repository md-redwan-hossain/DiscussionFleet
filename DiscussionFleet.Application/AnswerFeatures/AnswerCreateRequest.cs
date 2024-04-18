namespace DiscussionFleet.Application.AnswerFeatures;

public record AnswerCreateRequest(string Body, Guid QuestionId, Guid AnswerGiverId);