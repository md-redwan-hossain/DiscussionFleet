using DiscussionFleet.Domain;
using DiscussionFleet.Domain.Repositories;

namespace DiscussionFleet.Application;

public interface IApplicationUnitOfWork : IUnitOfWork
{
    public IMemberRepository MemberRepository { get; }
    public IBadgeRepository BadgeRepository { get; }
    public IBlogCategoryRepository BlogCategoryRepository { get; }
    public IBlogPostRepository BlogPostRepository { get; }
    public IForumRuleRepository ForumRuleRepository { get; }
    public IMultimediaImageRepository MultimediaImageRepository { get; }
    public IQuestionRepository QuestionRepository { get; }
    public ITagRepository TagRepository { get; }
    public IResourceNotificationRepository ResourceNotificationRepository { get; }
}