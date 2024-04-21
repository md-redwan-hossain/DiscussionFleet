using Autofac;
using DiscussionFleet.Application;
using DiscussionFleet.Application.Common.Services;
using DiscussionFleet.Application.Common.Utils;
using DiscussionFleet.Application.QuestionFeatures.DataTransferObjects;
using DiscussionFleet.Domain.Entities.QuestionAggregate.Utils;
using DiscussionFleet.Domain.Entities.TagAggregate;
using DiscussionFleet.Domain.Entities.UnaryAggregates;
using DiscussionFleet.Domain.Utils;
using DiscussionFleet.Web.Utils;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DiscussionFleet.Web.Models.QuestionWithRelated;

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
    public int CurrentPage { get; set; } = 1;
    [BindNever] public Paginator Pagination { get; set; }
    public string SearchText { get; set; }
    public bool LimitSearchForCurrentUser { get; set; }
    public HashSet<Guid> SelectedTags { get; set; } = [];
    public IList<SingleQuestionAtSearchResultViewModel> Questions { get; set; } = [];

    public async Task<IList<Tag>> FetchTagsAsync()
    {
        var data = await _appUnitOfWork.TagRepository.GetAllAsync(
            limit: 100000,
            orderBy: x => x.Id,
            useSplitQuery: false
        );

        return data;
    }


    public async Task FetchPostsAsync(string? currentUserId)
    {
        if (DataPerPage < 15) DataPerPage = 15;
        var (questions, total) = await _appUnitOfWork.QuestionRepository.GetQuestions(SortBy, FilterBy,
            SortOrder, CurrentPage, DataPerPage, SelectedTags, SearchText,
            currentUserId is not null && LimitSearchForCurrentUser ? Guid.Parse(currentUserId) : null
        );

        var pager = new Paginator(totalItems: total, dataPerPage: DataPerPage, currentPage: CurrentPage);

        Pagination = pager;

        foreach (var question in questions)
        {
            var tags = await _appUnitOfWork.TagRepository.GetAllAsync<string, Guid>(
                filter: x => question.Tags.Select(z => z.TagId).Contains(x.Id),
                subsetSelector: x => x.Title,
                orderBy: x => x.Id,
                useSplitQuery: false
            );
            var author = await _appUnitOfWork.MemberRepository.GetOneAsync(
                filter: x => x.Id == question.AuthorId,
                useSplitQuery: false
            );

            var answerCount = await _appUnitOfWork.AnswerRepository.GetCountAsync(x => x.QuestionId == question.Id);


            var shortTitle = question.Title.Length > 50 ? question.Title[..50] : question.Title;
            var shortBody = question.Body.Length > 200 ? question.Body[..200] : question.Body;

            shortBody = await _markdownService.MarkdownToPlainTextAsync(shortBody);

            var questionAtSearchResultViewModel = new SingleQuestionAtSearchResultViewModel
            {
                TitleResponse = new QuestionTitleResponse(shortTitle, question.Id),
                Body = shortBody,
                LastActivity = question.UpdatedAtUtc ?? question.CreatedAtUtc,
                AuthorName = author?.FullName ?? string.Empty,
                Tags = tags,
                Stats = new QuestionStatsViewModel
                {
                    HasAcceptedAnswer = question.HasAcceptedAnswer,
                    VoteCount = question.VoteCount,
                    AnswerCount = answerCount
                }
            };

            Questions.Add(questionAtSearchResultViewModel);
        }
    }

    public void Resolve(ILifetimeScope scope)
    {
        _scope = scope;
        _appUnitOfWork = _scope.Resolve<IApplicationUnitOfWork>();
        _markdownService = _scope.Resolve<IMarkdownService>();
    }
}