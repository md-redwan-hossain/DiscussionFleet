using Autofac;
using DiscussionFleet.Application;
using DiscussionFleet.Application.AnswerFeatures;
using DiscussionFleet.Application.Common.Options;
using DiscussionFleet.Application.Common.Services;
using DiscussionFleet.Application.ResourceNotificationFeatures;
using DiscussionFleet.Application.VotingFeatures;
using DiscussionFleet.Domain.Entities.ResourceNotificationAggregate;
using DiscussionFleet.Infrastructure.Identity.Services;
using DiscussionFleet.Web.Utils;
using Microsoft.Extensions.Options;

namespace DiscussionFleet.Web.Models.AnswerWithRelated;

public class AnswerViewModel : IViewModelWithResolve
{
    private ILifetimeScope _scope;
    private IApplicationUnitOfWork _appUnitOfWork;
    private IAnswerService _answerService;
    private IVotingService _votingService;
    private ForumRulesOptions _forumRulesOptions;
    private IResourceNotificationService _resourceNotificationService;

    public AnswerViewModel()
    {
    }


    public AnswerViewModel(IApplicationUnitOfWork appUnitOfWork, IAnswerService answerService,
        IVotingService votingService, IOptions<ForumRulesOptions> forumRulesOptions,
        IResourceNotificationService resourceNotificationService)
    {
        _appUnitOfWork = appUnitOfWork;
        _answerService = answerService;
        _votingService = votingService;
        _resourceNotificationService = resourceNotificationService;
        _forumRulesOptions = forumRulesOptions.Value;
    }

    public Guid Id { get; set; }
    public string Body { get; set; }


    public async Task<bool> IsQuestionExistsAsync(Guid answerGiverId, Guid questionId)
    {
        var question = await _appUnitOfWork.QuestionRepository.GetOneAsync(
            filter: x => x.Id == questionId,
            useSplitQuery: false
        );

        return question is not null;
    }

    public async Task ConductAnswerCreationAsync(Guid questionId, Guid answerGiverId)
    {
        await _answerService.CreateAsync(new AnswerCreateRequest(Body, questionId, answerGiverId));

        await _votingService.MemberReputationUpvoteAsync(answerGiverId, _forumRulesOptions.NewAnswer);

        var question = await _appUnitOfWork.QuestionRepository.GetOneAsync(
            filter: x => x.Id == questionId,
            useSplitQuery: false
        );

        if (question is not null)
        {
            await _resourceNotificationService.NotifyQuestionAuthorAsync(
                question.AuthorId, question.Id, question.Title,
                ResourceNotificationType.Answer
            );
        }
    }

    public void Resolve(ILifetimeScope scope)
    {
        _scope = scope;
        _appUnitOfWork = _scope.Resolve<IApplicationUnitOfWork>();
        _scope.Resolve<IMarkdownService>();
        _scope.Resolve<IMemberIdentityService>();
        _answerService = _scope.Resolve<IAnswerService>();
        _votingService = _scope.Resolve<IVotingService>();
        _forumRulesOptions = _scope.Resolve<IOptions<ForumRulesOptions>>().Value;
        _resourceNotificationService = _scope.Resolve<IResourceNotificationService>();
    }
}