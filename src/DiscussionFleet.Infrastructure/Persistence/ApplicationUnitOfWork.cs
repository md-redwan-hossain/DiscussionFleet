using DiscussionFleet.Application;
using DiscussionFleet.Domain.Repositories;

namespace DiscussionFleet.Infrastructure.Persistence;

public sealed class ApplicationUnitOfWork : UnitOfWork, IApplicationUnitOfWork
{
    public ApplicationUnitOfWork(ApplicationDbContext appDbContext,
        IMemberRepository memberRepository, IBadgeRepository badgeRepository,
        IForumRuleRepository forumRuleRepository, IMultimediaImageRepository multimediaImageRepository,
        IQuestionRepository questionRepository, ITagRepository tagRepository,
        IResourceNotificationRepository resourceNotificationRepository)
        : base(appDbContext)
    {
        MemberRepository = memberRepository;
        BadgeRepository = badgeRepository;
        ForumRuleRepository = forumRuleRepository;
        MultimediaImageRepository = multimediaImageRepository;
        QuestionRepository = questionRepository;
        TagRepository = tagRepository;
        ResourceNotificationRepository = resourceNotificationRepository;
    }

    public IMemberRepository MemberRepository { get; }
    public IBadgeRepository BadgeRepository { get; }
    public IForumRuleRepository ForumRuleRepository { get; }
    public IMultimediaImageRepository MultimediaImageRepository { get; }
    public IQuestionRepository QuestionRepository { get; }
    public ITagRepository TagRepository { get; }
    public IResourceNotificationRepository ResourceNotificationRepository { get; }
}