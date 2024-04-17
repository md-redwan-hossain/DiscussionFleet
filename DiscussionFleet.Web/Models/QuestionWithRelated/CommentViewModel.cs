namespace DiscussionFleet.Web.Models.QuestionWithRelated;

public class CommentViewModel
{
    public string AuthorName { get; set; }
    public string Body { get; set; }
    public int UsefulVoteCount { get; set; }
    // public Guid QuestionId { get; set; }
    // public Guid CommenterId { get; set; }
    public DateTime LastActivityUtc { get; set; }
}