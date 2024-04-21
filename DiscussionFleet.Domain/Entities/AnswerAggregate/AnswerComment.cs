using DiscussionFleet.Domain.Common;
using DiscussionFleet.Domain.Entities.CommentAggregate;

namespace DiscussionFleet.Domain.Entities.AnswerAggregate;

public class AnswerComment : Timestamp
{
    public AnswerId AnswerId { get; set; }
    public CommentId CommentId { get; set; }
}