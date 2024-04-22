using DiscussionFleet.Domain.Entities.MemberAggregate;
using DiscussionFleet.Domain.Entities.QuestionAggregate;

namespace DiscussionFleet.Application.AnswerFeatures;

public record AnswerCreateRequest(string Body, QuestionId QuestionId, MemberId AnswerGiverId);