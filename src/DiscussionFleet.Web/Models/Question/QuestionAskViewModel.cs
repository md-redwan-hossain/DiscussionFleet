using System.ComponentModel.DataAnnotations;
using Autofac;
using DiscussionFleet.Application;
using DiscussionFleet.Application.Common.Utils;
using DiscussionFleet.Application.QuestionFeatures.DataTransferObjects;
using DiscussionFleet.Application.QuestionFeatures.Services;
using DiscussionFleet.Application.TagFeatures;
using DiscussionFleet.Application.TagFeatures.DataTransferObjects;
using DiscussionFleet.Domain.Entities.Helpers;
using DiscussionFleet.Domain.Entities.UnaryAggregates;
using DiscussionFleet.Web.Utils;

namespace DiscussionFleet.Web.Models.Question;

public class QuestionAskViewModel : IViewModelWithResolve
{
    private ILifetimeScope _scope;
    private IApplicationUnitOfWork _appUnitOfWork;
    private ITagService _tagService;
    private IQuestionService _questionService;


    public QuestionAskViewModel()
    {
    }


    public QuestionAskViewModel(ILifetimeScope scope, IApplicationUnitOfWork appUnitOfWork, ITagService tagService,
        IQuestionService questionService)
    {
        _scope = scope;
        _appUnitOfWork = appUnitOfWork;
        _tagService = tagService;
        _questionService = questionService;
    }

    [Required]
    [StringLength(DomainEntityConstants.QuestionTitleMaxLength,
        ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
        MinimumLength = DomainEntityConstants.QuestionTitleMinLength)]
    [Display(Name = "Title")]
    public string Title { get; set; }

    [Required]
    // [Length(DomainEntityConstants.QuestionBodyMinLength,
    //     DomainEntityConstants.QuestionBodyMaxLength,
    //     ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
    [StringLength(DomainEntityConstants.QuestionBodyMaxLength,
        ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
        MinimumLength = DomainEntityConstants.QuestionBodyMinLength)]
    [Display(Name = "Body")]
    public string Body { get; set; }

    public bool HasError { get; set; }
    public bool CanCreate { get; set; } = true;
    public HashSet<Guid> SelectedTags { get; set; } = [];
    public HashSet<string> Tags { get; set; } = [];
    public byte MaxTags { get; set; } = 5;


    public async Task<string?> ConductAskQuestion(Guid id)
    {
        if (Tags.Count > 5) return "Maximum 5 tags are allowed";

        string? duplicateErrMsg = null;
        List<Guid> tagsId = [];

        if (Tags.Count > 0)
        {
            var tagCreateRequest = new TagCreateRequest(Tags);
            var outcome = await _tagService.CreateMany(tagCreateRequest);

            outcome.Switch(
                tags => tagsId.AddRange(tags.Select(x => x.Id)),
                err => duplicateErrMsg = Helpers.DelimitedCollection(err.DuplicateData)
            );

            if (duplicateErrMsg is not null)
            {
                return $"Duplicate tags: {duplicateErrMsg}";
            }
        }


        var questionCreateRequest = new QuestionCreateRequest(id, Title, Body, tagsId);
        await _questionService.CreateAsync(questionCreateRequest);
        return null;
    }


    public async Task<IList<Tag>> FetchTagsAsync()
    {
        var data = await _appUnitOfWork.TagRepository.GetAllAsync(limit: 100000, orderBy: x => x.Id);
        return data;
    }

    public void Resolve(ILifetimeScope scope)
    {
        _scope = scope;
        _appUnitOfWork = _scope.Resolve<IApplicationUnitOfWork>();
        _tagService = _scope.Resolve<ITagService>();
        _questionService = _scope.Resolve<IQuestionService>();
    }
}