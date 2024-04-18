using Autofac;
using DiscussionFleet.Application;
using DiscussionFleet.Application.Common.Options;
using DiscussionFleet.Application.Common.Services;
using DiscussionFleet.Application.QuestionFeatures.DataTransferObjects;
using DiscussionFleet.Domain.Entities.AnswerAggregate;
using DiscussionFleet.Domain.Entities.MemberAggregate;
using DiscussionFleet.Domain.Entities.QuestionAggregate;
using DiscussionFleet.Infrastructure.Identity.Services;
using DiscussionFleet.Web.Utils;
using Mapster;
using Microsoft.Extensions.Options;

namespace DiscussionFleet.Web.Models.QuestionWithRelated;

public class QuestionDetailsViewModel : IViewModelWithResolve
{
    private ILifetimeScope _scope;
    private IApplicationUnitOfWork _appUnitOfWork;
    private IMarkdownService _markdownService;
    private IMemberService _memberService;
    private ForumRulesOptions _forumRulesOptions;
    private LinkGenerator _linkGenerator;

    private IHttpContextAccessor _httpContextAccessor;

    public QuestionDetailsViewModel()
    {
    }

    public QuestionDetailsViewModel(IApplicationUnitOfWork appUnitOfWork, ILifetimeScope scope,
        IMarkdownService markdownService, IMemberService memberService,
        IOptions<ForumRulesOptions> forumRulesOptions, LinkGenerator linkGenerator,
        IHttpContextAccessor httpContextAccessor)
    {
        _appUnitOfWork = appUnitOfWork;
        _scope = scope;
        _markdownService = markdownService;
        _memberService = memberService;
        _linkGenerator = linkGenerator;
        _httpContextAccessor = httpContextAccessor;
        _forumRulesOptions = forumRulesOptions.Value;
    }


    public Guid Id { get; set; }

    // public Guid QuestionAuthorId { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public int VoteCount { get; set; }
    public int CommentCount { get; set; }
    public string AuthorName { get; set; }
    public string? ProfilePicUrl { get; set; }
    public HashSet<string> TagNames { get; set; } = [];
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public int AuthorReputation { get; set; }
    public int AnswerCount { get; set; }
    public bool CanUpvote { get; set; } = false;
    public bool CanDownVote { get; set; } = false;
    public bool CanMarkAsAccepted { get; set; } = false;
    public ICollection<ReadCommentViewModel> CommentsInQuestion { get; set; } = [];
    public ICollection<AnswerInQuestionViewModel> Answers { get; set; } = [];
    public ICollection<RelatedQuestionResponse> RelatedQuestionsByTag { get; set; } = [];
    public ICollection<Guid> RelatedQuestionTagIdCollection { get; set; } = [];

    public async Task<Question?> FetchQuestionAsync(Guid questionId)
    {
        var question = await _appUnitOfWork.QuestionRepository.GetOneAsync(
            filter: x => x.Id == questionId,
            includes: [a => a.Tags, b => b.Comments, c => c.AcceptedAnswer],
            useSplitQuery: true
        );
        return question;
    }

    public async Task<Member?> FetchAuthorAsync(Guid authorId)
    {
        return await _appUnitOfWork.MemberRepository.GetOneAsync(x => x.Id == authorId, useSplitQuery: false);
    }

    public async Task<bool> FetchQuestionRelatedDataAsync(Question question,
        Member author)
    {
        var data = await _memberService.GetCachedMemberInfoAsync(author.Id.ToString());
        ProfilePicUrl = data?.ProfileImageUrl;

        await question.BuildAdapter().AdaptToAsync(this);
        AuthorName = author.FullName;
        AuthorReputation = author.ReputationCount;
        UpdatedAtUtc ??= CreatedAtUtc;

        var tags = await _appUnitOfWork.TagRepository.GetAllAsync(
            filter: x => question.Tags.Select(z => z.TagId).Contains(x.Id),
            subsetSelector: x => new { x.Id, x.Title },
            orderBy: x => x.Id,
            useSplitQuery: false
        );

        foreach (var tag in tags)
        {
            RelatedQuestionTagIdCollection.Add(tag.Id);
            TagNames.Add(tag.Title);
        }


        Body = await _markdownService.MarkdownToHtmlAsync(Body);
        Body = await _markdownService.SanitizeConvertedHtmlAsync(Body);


        await LoadQuestionCommentsAsync(question.Comments);
        await LoadAnswersAsync(question, author);

        return true;
    }


    public async Task<ICollection<RelatedQuestionResponse>> LoadRelatedQuestionsByTag(ICollection<Guid> tags)
    {
        ICollection<RelatedQuestionResponse> storage = [];

        if (tags.Count == 0) return storage;

        var questions = await _appUnitOfWork.QuestionRepository.GetAllAsync(
            filter: x => x.Tags.Any(t => tags.Contains(t.TagId)),
            orderBy: x => x.VoteCount,
            limit: 10,
            useSplitQuery: false
        );

        foreach (var item in questions)
        {
            if (_httpContextAccessor.HttpContext is null) continue;

            var url = _linkGenerator.GetUriByAction(
                _httpContextAccessor.HttpContext,
                "Details", "Questions",
                new { id = item.Id }
            );

            if (url is null) continue;
            
            var dto = new RelatedQuestionResponse(item.Id, item.Title, item.VoteCount,
                url, item.UpdatedAtUtc ?? item.CreatedAtUtc);

            storage.Add(dto);
        }

        return storage;
    }

    private async Task LoadQuestionCommentsAsync(ICollection<QuestionComment> questionComments)
    {
        var comments = await _appUnitOfWork.CommentRepository.GetAllAsync(
            filter: x => questionComments.Select(z => z.CommentId).Contains(x.Id),
            orderBy: x => x.Id,
            useSplitQuery: false
        );

        var questionCommenters = await _appUnitOfWork.MemberRepository.GetAllAsync(
            filter: x => comments.Select(z => z.CommenterId).Contains(x.Id),
            orderBy: x => x.Id,
            useSplitQuery: false
        );

        foreach (var comment in comments)
        {
            var ansAuthor = questionCommenters.FirstOrDefault(x => x.Id == comment.CommenterId);
            if (ansAuthor is null) continue;


            var commentViewModel = new ReadCommentViewModel
            {
                AuthorName = ansAuthor.FullName,
                LastActivityUtc = comment.UpdatedAtUtc ?? comment.CreatedAtUtc
            };

            await comment.BuildAdapter().AdaptToAsync(commentViewModel);
            CommentsInQuestion.Add(commentViewModel);
        }
    }

    private async Task LoadAnswersAsync(Question question, Member author)
    {
        var answers = await _appUnitOfWork.AnswerRepository.GetAllAsync(
            filter: x =>  x.QuestionId == question.Id,
            orderBy: x => x.Id,
            includes: [x => x.Comments],
            useSplitQuery: false
        );

        var ansAuthors = await _appUnitOfWork.MemberRepository.GetAllAsync(
            filter: x => answers.Select(z => z.AnswerGiverId).Contains(x.Id),
            orderBy: x => x.Id,
            useSplitQuery: false
        );


        foreach (var answer in answers)
        {
            var ansInQnViewModel = new AnswerInQuestionViewModel
            {
                Id = answer.Id,
                Body = answer.Body,
                VoteCount = answer.VoteCount,
                CommentCount = answer.CommentCount,
                CreatedAtUtc = answer.CreatedAtUtc,
                UpdatedAtUtc = answer.UpdatedAtUtc ?? answer.CreatedAtUtc
            };

            if (question.AcceptedAnswer is not null && answer.Id == question.AcceptedAnswer.AnswerId)
            {
                ansInQnViewModel.IsAccepted = true;
            }

            var pickedAnsAuthor = ansAuthors.FirstOrDefault(x => x.Id == answer.AnswerGiverId);
            if (pickedAnsAuthor is null) continue;

            ansInQnViewModel.AuthorName = pickedAnsAuthor.FullName;
            ansInQnViewModel.AuthorReputation = pickedAnsAuthor.ReputationCount;

            var pickedAnsAuthorCache = await _memberService.GetCachedMemberInfoAsync(answer.AnswerGiverId.ToString());
            if (pickedAnsAuthorCache is not null)
            {
                ansInQnViewModel.ProfilePicUrl = pickedAnsAuthorCache.ProfileImageUrl;
            }

            ansInQnViewModel.CommentsInAnswer = await LoadAnswerCommentsAsync(answer.Comments);
            Answers.Add(ansInQnViewModel);
        }
    }

    private async Task<ICollection<ReadCommentViewModel>> LoadAnswerCommentsAsync(
        ICollection<AnswerComment> answerComments)
    {
        ICollection<ReadCommentViewModel> cvmStorage = [];


        var comments = await _appUnitOfWork.CommentRepository.GetAllAsync(
            filter: x => answerComments.Select(z => z.CommentId).Contains(x.Id),
            orderBy: x => x.Id,
            useSplitQuery: false
        );


        var answerCommenters = await _appUnitOfWork.MemberRepository.GetAllAsync(
            filter: x => comments.Select(z => z.CommenterId).Contains(x.Id),
            orderBy: x => x.Id,
            useSplitQuery: false
        );

        foreach (var comment in comments)
        {
            var ansAuthor = answerCommenters.FirstOrDefault(x => x.Id == comment.CommenterId);
            if (ansAuthor is null) continue;

            var commentViewModel = new ReadCommentViewModel
            {
                AuthorName = ansAuthor.FullName,
                LastActivityUtc = comment.UpdatedAtUtc ?? comment.CreatedAtUtc
            };

            await comment.BuildAdapter().AdaptToAsync(commentViewModel);

            cvmStorage.Add(commentViewModel);
        }

        return cvmStorage;
    }


    public void Resolve(ILifetimeScope scope)
    {
        _scope = scope;
        _appUnitOfWork = _scope.Resolve<IApplicationUnitOfWork>();
        _markdownService = _scope.Resolve<IMarkdownService>();
        _memberService = _scope.Resolve<IMemberService>();
        _linkGenerator = _scope.Resolve<LinkGenerator>();
        _httpContextAccessor = _scope.Resolve<IHttpContextAccessor>();
        _forumRulesOptions = _scope.Resolve<IOptions<ForumRulesOptions>>().Value;
    }
}