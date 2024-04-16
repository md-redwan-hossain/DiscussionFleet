using Autofac;
using DiscussionFleet.Application;
using DiscussionFleet.Application.Common.Services;
using DiscussionFleet.Infrastructure.Identity.Services;
using DiscussionFleet.Web.Utils;
using Mapster;

namespace DiscussionFleet.Web.Models.Question;

public class QuestionDetailsViewModel : IViewModelWithResolve
{
    private ILifetimeScope _scope;
    private IApplicationUnitOfWork _appUnitOfWork;
    private IMarkdownService _markdownService;
    private IMemberService _memberService;

    public QuestionDetailsViewModel()
    {
    }

    public QuestionDetailsViewModel(IApplicationUnitOfWork appUnitOfWork, ILifetimeScope scope,
        IMarkdownService markdownService, IMemberService memberService)
    {
        _appUnitOfWork = appUnitOfWork;
        _scope = scope;
        _markdownService = markdownService;
        _memberService = memberService;
    }


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


    public async Task<bool> FetchQuestion(Guid questionId)
    {
        var question = await _appUnitOfWork.QuestionRepository.GetOneAsync(
            filter: x => x.Id == questionId,
            includes: [x => x.Tags, x => x.Comments]
        );
        if (question is null) return false;

        var author = await _appUnitOfWork.MemberRepository.GetOneAsync(x => x.Id == question.AuthorId);

        if (author is null) return false;


        var data = await _memberService.GetCachedMemberInfoAsync(author.Id.ToString());
        ProfilePicUrl = data?.ProfileImageUrl;

        await question.BuildAdapter().AdaptToAsync(this);
        AuthorName = author.FullName;
        AuthorReputation = author.ReputationCount;
        UpdatedAtUtc ??= CreatedAtUtc;

        var tags = await _appUnitOfWork.TagRepository.GetAllAsync<string, Guid>(
            filter: x => question.Tags.Select(z => z.TagId).Contains(x.Id),
            subsetSelector: x => x.Title,
            orderBy: x => x.Id
        );

        TagNames = [..tags];

        Body = await _markdownService.MarkdownToHtmlAsync(Body);
        Body = await _markdownService.SanitizeConvertedHtmlAsync(Body);


        // Title = question.Title;
        // Body = question.Body;
        // CreatedAtUtc = question.CreatedAtUtc;


        return true;
    }

    public void Resolve(ILifetimeScope scope)
    {
        _scope = scope;
        _appUnitOfWork = _scope.Resolve<IApplicationUnitOfWork>();
        _markdownService = _scope.Resolve<IMarkdownService>();
        _memberService = _scope.Resolve<IMemberService>();
    }
}