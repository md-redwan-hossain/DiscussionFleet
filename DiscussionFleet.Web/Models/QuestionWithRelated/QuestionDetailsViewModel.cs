using Autofac;
using DiscussionFleet.Application;
using DiscussionFleet.Application.Common.Services;
using DiscussionFleet.Domain.Entities.AnswerAggregate;
using DiscussionFleet.Domain.Entities.MemberAggregate;
using DiscussionFleet.Domain.Entities.QuestionAggregate;
using DiscussionFleet.Infrastructure.Identity.Services;
using DiscussionFleet.Web.Utils;
using Mapster;

namespace DiscussionFleet.Web.Models.QuestionWithRelated;

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
    public bool CanVote { get; set; }
    public ICollection<CommentViewModel> Comments { get; set; } = [];
    public ICollection<AnswerInQuestionViewModel> Answers { get; set; } = [];


    public async Task<bool> FetchQuestion(Guid questionId)
    {
        var question = await _appUnitOfWork.QuestionRepository.GetOneAsync(
            filter: x => x.Id == questionId,
            includes: [a => a.Tags, b => b.Comments, c => c.AcceptedAnswer]
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


        await LoadQuestionCommentsAsync(question.Comments);
        await LoadAnswersAsync(question, author);

        return true;
    }


    private async Task LoadQuestionCommentsAsync(ICollection<QuestionComment> questionComments)
    {
        var questionCommenters = await _appUnitOfWork.MemberRepository.GetAllAsync(
            filter: x => questionComments.Select(z => z.CommenterId).Contains(x.Id),
            orderBy: x => x.Id
        );

        foreach (var comment in questionComments)
        {
            var ansAuthor = questionCommenters.FirstOrDefault(x => x.Id == comment.CommenterId);
            if (ansAuthor is null) continue;


            var commentViewModel = new CommentViewModel
            {
                AuthorName = ansAuthor.FullName,
                LastActivityUtc = comment.UpdatedAtUtc ?? comment.CreatedAtUtc
            };

            await comment.BuildAdapter().AdaptToAsync(commentViewModel);
            Comments.Add(commentViewModel);
        }
    }


    private async Task LoadAnswersAsync(Question question, Member author)
    {
        var answers = await _appUnitOfWork.AnswerRepository.GetAllAsync(
            filter: x => question.AuthorId == author.Id,
            orderBy: x => x.Id,
            includes: [x => x.Comments]
        );

        var ansAuthors = await _appUnitOfWork.MemberRepository.GetAllAsync(
            filter: x => answers.Select(z => z.AnswerGiverId).Contains(x.Id),
            orderBy: x => x.Id
        );


        foreach (var answer in answers)
        {
            var ansInQnViewModel = new AnswerInQuestionViewModel
            {
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

            var pickedAnsAuthorCache = await _memberService.GetCachedMemberInfoAsync(author.Id.ToString());
            if (pickedAnsAuthorCache is not null)
            {
                ansInQnViewModel.ProfilePicUrl = pickedAnsAuthorCache.ProfileImageUrl;
            }

            ansInQnViewModel.Comments = await LoadAnswerCommentsAsync(answer.Comments);
            Answers.Add(ansInQnViewModel);
        }
    }


    private async Task<ICollection<CommentViewModel>> LoadAnswerCommentsAsync(ICollection<AnswerComment> answerComments)
    {
        ICollection<CommentViewModel> cvmStorage = [];

        var answerCommenters = await _appUnitOfWork.MemberRepository.GetAllAsync(
            filter: x => answerComments.Select(z => z.CommenterId).Contains(x.Id),
            orderBy: x => x.Id
        );

        foreach (var comment in answerComments)
        {
            var ansAuthor = answerCommenters.FirstOrDefault(x => x.Id == comment.CommenterId);
            if (ansAuthor is null) continue;

            var commentViewModel = new CommentViewModel
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
    }
}