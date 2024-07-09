namespace DiscussionFleet.Web.Models.QuestionWithRelated;

public class QuestionStatsViewModel
{
    public int VoteCount { get; set; }
    public int AnswerCount { get; set; }
    public bool HasAcceptedAnswer { get; set; }
}