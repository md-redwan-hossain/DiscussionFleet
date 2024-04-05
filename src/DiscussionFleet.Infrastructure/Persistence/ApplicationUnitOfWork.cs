using DiscussionFleet.Application;
using DiscussionFleet.Domain.Repositories;

namespace DiscussionFleet.Infrastructure.Persistence;

public sealed class ApplicationUnitOfWork : UnitOfWork, IApplicationUnitOfWork
{
    public ApplicationUnitOfWork(ApplicationDbContext dbContext,
        IMemberRepository memberRepository, IBadgeRepository badgeRepository,
        IBlogCategoryRepository blogCategoryRepository, IBlogPostRepository blogPostRepository,
        IForumRuleRepository forumRuleRepository, IMultimediaImageRepository multimediaImageRepository,
        IQuestionRepository questionRepository, ITagRepository tagRepository,
        IResourceNotificationRepository resourceNotificationRepository)
        : base(dbContext)
    {
        MemberRepository = memberRepository;
        BadgeRepository = badgeRepository;
        BlogCategoryRepository = blogCategoryRepository;
        BlogPostRepository = blogPostRepository;
        ForumRuleRepository = forumRuleRepository;
        MultimediaImageRepository = multimediaImageRepository;
        QuestionRepository = questionRepository;
        TagRepository = tagRepository;
        ResourceNotificationRepository = resourceNotificationRepository;
    }

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