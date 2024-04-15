using DiscussionFleet.Domain.Repositories;
using DiscussionFleet.Domain.Utils;

namespace DiscussionFleet.Application;

public interface IApplicationUnitOfWork : IUnitOfWork
{
    public IMemberRepository MemberRepository { get; }
    public IBadgeRepository BadgeRepository { get; }
    public IForumRuleRepository ForumRuleRepository { get; }
    public IMultimediaImageRepository MultimediaImageRepository { get; }
    public IQuestionRepository QuestionRepository { get; }
    public IAnswerRepository AnswerRepository { get; }
    public ITagRepository TagRepository { get; }
    public IResourceNotificationRepository ResourceNotificationRepository { get; }
}