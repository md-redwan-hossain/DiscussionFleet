using DiscussionFleet.Application;
using DiscussionFleet.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DiscussionFleet.Infrastructure.Persistence;

public sealed class ApplicationUnitOfWork : UnitOfWork, IApplicationUnitOfWork
{
    public ApplicationUnitOfWork(IApplicationDbContext dbContext,
        IMemberRepository memberRepository, IBadgeRepository badgeRepository,
        IBlogCategoryRepository blogCategoryRepository, IBlogPostRepository blogPostRepository,
        IForumRuleRepository forumRuleRepository, IMultimediaImageRepository multimediaImageRepository,
        IQuestionRepository questionRepository, ITagRepository tagRepository)
        : base((DbContext)dbContext)
    {
        MemberRepository = memberRepository;
        BadgeRepository = badgeRepository;
        BlogCategoryRepository = blogCategoryRepository;
        BlogPostRepository = blogPostRepository;
        ForumRuleRepository = forumRuleRepository;
        MultimediaImageRepository = multimediaImageRepository;
        QuestionRepository = questionRepository;
        TagRepository = tagRepository;
    }

    public IMemberRepository MemberRepository { get; }
    public IBadgeRepository BadgeRepository { get; }
    public IBlogCategoryRepository BlogCategoryRepository { get; }
    public IBlogPostRepository BlogPostRepository { get; }
    public IForumRuleRepository ForumRuleRepository { get; }
    public IMultimediaImageRepository MultimediaImageRepository { get; }
    public IQuestionRepository QuestionRepository { get; }
    public ITagRepository TagRepository { get; }
}