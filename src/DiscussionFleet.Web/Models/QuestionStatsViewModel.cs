namespace DiscussionFleet.Web.Models;

public class QuestionStatsViewModel
{
    public int VoteCount { get; set; }
    public int AnswerCount { get; set; }
    public long ViewCount { get; set; }
    public bool HasAcceptedAnswer { get; set; }
}