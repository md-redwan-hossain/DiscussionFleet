namespace DiscussionFleet.Web.Models.QuestionWithRelated;

public class AnswerInQuestionViewModel
{
    public Guid Id { get; set; }
    public string Body { get; set; }
    public int VoteCount { get; set; }
    public int CommentCount { get; set; }
    public string AuthorName { get; set; }
    public string? ProfilePicUrl { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public int AuthorReputation { get; set; }
    public bool IsAccepted { get; set; }
    public bool CanUpvote { get; set; } = false;
    public bool CanDownVote { get; set; } = false;
    public ICollection<ReadCommentViewModel> CommentsInAnswer { get; set; } = [];
}