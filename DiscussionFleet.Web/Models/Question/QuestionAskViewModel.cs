using System.ComponentModel.DataAnnotations;
using Autofac;
using DiscussionFleet.Application;
using DiscussionFleet.Application.Common.Options;
using DiscussionFleet.Application.MemberReputationFeatures;
using DiscussionFleet.Application.QuestionFeatures.DataTransferObjects;
using DiscussionFleet.Application.QuestionFeatures.Services;
using DiscussionFleet.Application.TagFeatures.DataTransferObjects;
using DiscussionFleet.Domain.Entities.Helpers;
using DiscussionFleet.Domain.Entities.UnaryAggregates;
using DiscussionFleet.Web.Utils;
using Microsoft.Extensions.Options;
using SharpOutcome;

namespace DiscussionFleet.Web.Models.Question;

public class QuestionAskViewModel : IViewModelWithResolve
{
    private ILifetimeScope _scope;
    private IApplicationUnitOfWork _appUnitOfWork;
    private IQuestionService _questionService;


    public QuestionAskViewModel()
    {
    }


    public QuestionAskViewModel(ILifetimeScope scope, IApplicationUnitOfWork appUnitOfWork,
        IQuestionService questionService)
    {
        _scope = scope;
        _appUnitOfWork = appUnitOfWork;
        _questionService = questionService;
    }

    [Required]
    [StringLength(DomainEntityConstants.QuestionTitleMaxLength,
        ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
        MinimumLength = DomainEntityConstants.QuestionTitleMinLength)]
    [Display(Name = "Title")]
    public string Title { get; set; }

    [Required]
    [StringLength(DomainEntityConstants.QuestionBodyMaxLength,
        ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
        MinimumLength = DomainEntityConstants.QuestionBodyMinLength)]
    [Display(Name = "Body")]
    public string Body { get; set; }

    public bool HasError { get; set; }
    public bool CanCreate { get; set; } = true;
    public HashSet<Guid>? SelectedExistingTags { get; set; } = [];
    public HashSet<string> NewCreatedTags { get; set; } = [];
    public byte MaxTags { get; set; } = 5;


    public async Task<string?> ConductAskQuestion(Guid id)
    {
        if (SelectedExistingTags is null && NewCreatedTags.Count == 0) return "At least 1 tag is required.";

        if (NewCreatedTags.Count + SelectedExistingTags?.Count == 0) return "At least 1 tag is required.";

        if (NewCreatedTags.Count + SelectedExistingTags?.Count > 5) return "Maximum 5 tags are allowed.";

        var dto = new QuestionWithNewTagsCreateRequest(id, Title, Body, SelectedExistingTags ?? [],
            new TagCreateRequest(NewCreatedTags));

        var outcome = await _questionService.CreateWithNewTagsAsync(dto);

        if (outcome.TryPickBadOutcome(out var err))
        {
            return err.Tag is BadOutcomeTag.Duplicate
                ? $"Duplicate tags: {err}"
                : "Unknown Error";
        }

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
        _questionService = _scope.Resolve<IQuestionService>();
    }
}