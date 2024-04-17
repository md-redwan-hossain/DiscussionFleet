using DiscussionFleet.Application;
using DiscussionFleet.Domain.Repositories;

namespace DiscussionFleet.Infrastructure.Persistence;

public sealed class ApplicationUnitOfWork : UnitOfWork, IApplicationUnitOfWork
{
    public ApplicationUnitOfWork(ApplicationDbContext appDbContext,
        IMemberRepository memberRepository, IMultimediaImageRepository multimediaImageRepository,
        IQuestionRepository questionRepository, ITagRepository tagRepository,
        IResourceNotificationRepository resourceNotificationRepository, IAnswerRepository answerRepository,
        IQuestionVoteRepository questionVoteRepository, IAnswerVoteRepository answerVoteRepository,
        ICommentRepository commentRepository)
        : base(appDbContext)
    {
        MemberRepository = memberRepository;
        MultimediaImageRepository = multimediaImageRepository;
        QuestionRepository = questionRepository;
        TagRepository = tagRepository;
        ResourceNotificationRepository = resourceNotificationRepository;
        AnswerRepository = answerRepository;
        QuestionVoteRepository = questionVoteRepository;
        AnswerVoteRepository = answerVoteRepository;
        CommentRepository = commentRepository;
    }

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