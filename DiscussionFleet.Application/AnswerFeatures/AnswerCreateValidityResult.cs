namespace DiscussionFleet.Application.AnswerFeatures;

public enum AnswerCreateValidityResult : byte
{
    QuestionNotFound = 1,
    HomogenousUser,
    Valid
}