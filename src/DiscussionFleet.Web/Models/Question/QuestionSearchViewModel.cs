using Autofac;
using DiscussionFleet.Application;
using DiscussionFleet.Application.Common.Services;
using DiscussionFleet.Application.Common.Utils;
using DiscussionFleet.Application.QuestionFeatures.DataTransferObjects;
using DiscussionFleet.Domain.Entities.QuestionAggregate.Utils;
using DiscussionFleet.Domain.Entities.UnaryAggregates;
using DiscussionFleet.Domain.Utils;
using DiscussionFleet.Web.Utils;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DiscussionFleet.Web.Models.Question;

public class QuestionSearchViewModel : IViewModelWithResolve
{
    private ILifetimeScope _scope;
    private IApplicationUnitOfWork _appUnitOfWork;
    private IMarkdownService _markdownService;

    public QuestionSearchViewModel()
    {
    }

    public QuestionSearchViewModel(IApplicationUnitOfWork appUnitOfWork, ILifetimeScope scope,
        IMarkdownService markdownService)
    {
        _appUnitOfWork = appUnitOfWork;
        _scope = scope;
        _markdownService = markdownService;
    }

    public DataSortOrder SortOrder { get; set; } = DataSortOrder.Asc;
    public QuestionFilterCriteria FilterBy { get; set; } = new();
    public QuestionSortCriteria SortBy { get; set; } = QuestionSortCriteria.Newest;
    public byte DataPerPage { get; set; } = 15;
    public int TotalData { get; set; }
    public int CurrentPage { get; set; } = 1;
    [BindNever] public Paginator Pagination { get; set; }
    public HashSet<Guid> SelectedTags { get; set; } = [];
    public IList<QuestionAtSearchResultViewModel> Questions { get; set; } = [];

    public async Task<IList<Tag>> FetchTagsAsync()
    {
        var data = await _appUnitOfWork.TagRepository.GetAllAsync(limit: 100000, orderBy: x => x.Id);
        return data;
    }


    public async Task FetchPostsAsync()
    {
        if (DataPerPage < 15) DataPerPage = 15;
        var (enumerable, total) = await _appUnitOfWork.QuestionRepository.GetQuestions(SortBy, FilterBy,
            SortOrder, CurrentPage, DataPerPage, SelectedTags);

        var pager = new Paginator(totalItems: total, dataPerPage: DataPerPage, currentPage: CurrentPage);

        Pagination = pager;

        foreach (var question in enumerable)
        {
            var tags = await _appUnitOfWork.TagRepository.GetAllAsync<string, Guid>(
                filter: x => question.Tags.Select(z => z.TagId).Contains(x.Id),
                subsetSelector: x => x.Title,
                orderBy: x => x.Id
            );
            var author = await _appUnitOfWork.MemberRepository.GetOneAsync(x => x.Id == question.AuthorId);
            var shortTitle = question.Title.Length > 50 ? question.Title[..50] : question.Title;
            var shortBody = question.Body.Length > 200 ? question.Body[..200] : question.Body;

            shortBody = await _markdownService.MarkdownToPlainText(shortBody);
            
            var q = new QuestionAtSearchResultViewModel
            {
                TitleResponse = new QuestionTitleResponse(shortTitle, string.Empty),
                Body = shortBody,
                LastActivity = question.UpdatedAtUtc ?? question.CreatedAtUtc,
                AuthorResponse = new QuestionAuthorResponse(author?.FullName ?? string.Empty, string.Empty),
                Tags = tags,
                Stats = new QuestionStatsViewModel
                {
                    HasAcceptedAnswer = question.HasAcceptedAnswer,
                    VoteCount = question.VoteCount,
                    AnswerCount = 2
                }
            };

            Questions.Add(q);
        }
    }

    public void Resolve(ILifetimeScope scope)
    {
        _scope = scope;
        _appUnitOfWork = _scope.Resolve<IApplicationUnitOfWork>();
        _markdownService = _scope.Resolve<IMarkdownService>();
    }
}