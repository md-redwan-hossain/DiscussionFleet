using Autofac;
using DiscussionFleet.Application;
using DiscussionFleet.Application.AnswerFeatures;
using DiscussionFleet.Application.Common.Options;
using DiscussionFleet.Application.Common.Services;
using DiscussionFleet.Application.VotingFeatures;
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

    public AnswerViewModel()
    {
    }


    public AnswerViewModel(IApplicationUnitOfWork appUnitOfWork, IAnswerService answerService,
        IVotingService votingService, IOptions<ForumRulesOptions> forumRulesOptions)
    {
        _appUnitOfWork = appUnitOfWork;
        _answerService = answerService;
        _votingService = votingService;
        _forumRulesOptions = forumRulesOptions.Value;
    }

    public string Body { get; set; }


    public async Task<AnswerCreateValidityResult> CheckValidAuthorAsync(Guid answerGiverId, Guid questionId)
    {
        var question = await _appUnitOfWork.QuestionRepository.GetOneAsync(x => x.Id == questionId);
        if (question is null) return AnswerCreateValidityResult.QuestionNotFound;

        return question.AuthorId == answerGiverId
            ? AnswerCreateValidityResult.HomogenousUser
            : AnswerCreateValidityResult.Valid;
    }

    public async Task ConductAnswerCreationAsync(Guid answerGiverId)
    {
        await _answerService.CreateAsync(new AnswerCreateRequest(Body, answerGiverId));

        await _votingService.MemberUpvoteAsync(answerGiverId, _forumRulesOptions.NewAnswer);
    }

    public void Resolve(ILifetimeScope scope)
    {
        _scope = scope;
        _appUnitOfWork = _scope.Resolve<IApplicationUnitOfWork>();
        _scope.Resolve<IMarkdownService>();
        _scope.Resolve<IMemberService>();
        _answerService = _scope.Resolve<IAnswerService>();
        _votingService = _scope.Resolve<IVotingService>();
        _forumRulesOptions = _scope.Resolve<IOptions<ForumRulesOptions>>().Value;
    }
}