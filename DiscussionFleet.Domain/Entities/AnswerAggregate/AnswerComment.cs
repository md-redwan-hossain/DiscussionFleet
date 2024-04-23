using DiscussionFleet.Domain.Common;
using DiscussionFleet.Domain.Entities.CommentAggregate;

namespace DiscussionFleet.Domain.Entities.AnswerAggregate;

public class AnswerComment : Timestamp
{
    public required AnswerId AnswerId { get; set; }
    public required CommentId CommentId { get; set; }
}