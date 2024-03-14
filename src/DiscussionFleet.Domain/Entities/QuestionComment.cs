using DiscussionFleet.Domain.Entities.Abstracts;

namespace DiscussionFleet.Domain.Entities;

public class QuestionComment : Timestamp
{
    public long QuestionId { get; set; }
    public long CommenterId { get; set; }
    public string Body { get; set; }
    public int UsefulVoteCount { get; set; }
    public int EditCount { get; set; }
}