using DiscussionFleet.Domain.Repositories;
using DiscussionFleet.Domain.Utils;

namespace DiscussionFleet.Application;

public interface IApplicationUnitOfWork : IUnitOfWork
{
    public IMemberRepository MemberRepository { get; }
    public IQuestionVoteRepository QuestionVoteRepository { get; }
    public IAnswerVoteRepository AnswerVoteRepository { get; }
    public ICommentRepository CommentRepository { get; }
    public IMultimediaImageRepository MultimediaImageRepository { get; }
    public IQuestionRepository QuestionRepository { get; }
    public IAnswerRepository AnswerRepository { get; }
    public ITagRepository TagRepository { get; }
    public IResourceNotificationRepository ResourceNotificationRepository { get; }
}