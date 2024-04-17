using Autofac;
using DiscussionFleet.Application;
using DiscussionFleet.Application.AnswerFeatures;
using DiscussionFleet.Application.Common.Options;
using DiscussionFleet.Application.Common.Services;
using DiscussionFleet.Application.MemberReputationFeatures;
using DiscussionFleet.Infrastructure.Identity.Services;
using DiscussionFleet.Web.Utils;
using Microsoft.Extensions.Options;

namespace DiscussionFleet.Web.Models.AnswerWithRelated;

public class AnswerViewModel : IViewModelWithResolve
{
    private ILifetimeScope _scope;
    private IApplicationUnitOfWork _appUnitOfWork;
    private IAnswerService _answerService;
    private IMemberReputationService _memberReputationService;
    private ForumRulesOptions _forumRulesOptions;

    public AnswerViewModel()
    {
    }


    public AnswerViewModel(IApplicationUnitOfWork appUnitOfWork, IAnswerService answerService,
        IMemberReputationService memberReputationService, IOptions<ForumRulesOptions> forumRulesOptions)
    {
        _appUnitOfWork = appUnitOfWork;
        _answerService = answerService;
        _memberReputationService = memberReputationService;
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

        await _memberReputationService.UpvoteAsync(answerGiverId, _forumRulesOptions.NewAnswer);
    }

    public void Resolve(ILifetimeScope scope)
    {
        _scope = scope;
        _appUnitOfWork = _scope.Resolve<IApplicationUnitOfWork>();
        _scope.Resolve<IMarkdownService>();
        _scope.Resolve<IMemberService>();
        _answerService = _scope.Resolve<IAnswerService>();
        _memberReputationService = _scope.Resolve<IMemberReputationService>();
        _forumRulesOptions = _scope.Resolve<IOptions<ForumRulesOptions>>().Value;
    }
}