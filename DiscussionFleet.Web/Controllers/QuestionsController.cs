using System.Security.Claims;
using Autofac;
using DiscussionFleet.Application.AnswerFeatures;
using DiscussionFleet.Application.Common.Options;
using DiscussionFleet.Application.VotingFeatures;
using DiscussionFleet.Domain.Entities.AnswerAggregate.Utils;
using DiscussionFleet.Domain.Entities.QuestionAggregate.Utils;
using DiscussionFleet.Web.Models.AnswerWithRelated;
using DiscussionFleet.Web.Models.Others;
using DiscussionFleet.Web.Models.QuestionWithRelated;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscussionFleet.Web.Controllers;

public class QuestionsController : Controller
{
    private readonly ILifetimeScope _scope;
    private readonly IVotingService _votingService;
    private readonly IAnswerService _answerService;

    public QuestionsController(ILifetimeScope scope, IVotingService votingService, IAnswerService answerService)
    {
        _scope = scope;
        _votingService = votingService;
        _answerService = answerService;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] QuestionSearchViewModel viewModel)
    {
        viewModel.Resolve(_scope);

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        await viewModel.FetchPostsAsync(currentUserId);
        return View(viewModel);
    }


    [HttpGet]
    public async Task<IActionResult> Details(Guid id, [FromQuery] int page,
        [FromQuery] AnswerSortCriteria sortBy = AnswerSortCriteria.HighestScore)
    {
        var viewModel = _scope.Resolve<QuestionDetailsViewModel>();
        var question = await viewModel.FetchQuestionAsync(id);
        if (question is null) return RedirectToAction(nameof(Index));

        var author = await viewModel.FetchAuthorAsync(question.AuthorId);
        if (author is null) return RedirectToAction(nameof(Index));


        var result = await viewModel.FetchQuestionRelatedDataAsync(question, author);
        if (result is false) return RedirectToAction(nameof(Index));


        viewModel.Answers = await viewModel.FetchAnswersAsync(question, page, sortBy);


        viewModel.RelatedQuestionsByTag =
            await viewModel.LoadRelatedQuestionsByTag(viewModel.RelatedQuestionTagIdCollection);

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var canVoteQuestionResult = await _votingService
            .CheckQuestionVotingAbilityAsync(currentUserId, author.Id, question.Id);

        if (currentUserId is not null && Guid.Parse(currentUserId) == question.AuthorId)
        {
            viewModel.CanMarkAsAccepted = true;
        }

        viewModel.CanUpvote = canVoteQuestionResult.upvote;
        viewModel.CanDownVote = canVoteQuestionResult.downVote;

        foreach (var ans in viewModel.Answers)
        {
            var canVoteAnswerResult = await _votingService
                .CheckAnswerVotingAbilityAsync(currentUserId, ans.AnswerGiverId, ans.Id);

            ans.CanUpvote = canVoteAnswerResult.upvote;
            ans.CanDownVote = canVoteAnswerResult.downVote;
        }

        return View(viewModel);
    }


    [HttpGet, Authorize]
    public IActionResult Ask()
    {
        var model = _scope.Resolve<QuestionAskViewModel>();
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize]
    public async Task<IActionResult> Ask(QuestionAskViewModel viewModel)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (currentUserId is null)
        {
            return RedirectToAction("Login", "Account");
        }

        if (ModelState.IsValid is false)
        {
            return View(viewModel);
        }

        viewModel.Resolve(_scope);
        var outcome = await viewModel.ConductAskQuestion(Guid.Parse(currentUserId));

        if (outcome.TryPickBadOutcome(out var err))
        {
            viewModel.HasError = true;
            ModelState.AddModelError(string.Empty, err);
            return View(viewModel);
        }

        outcome.TryPickGoodOutcome(out var id);

        return RedirectToAction(nameof(Details), new { id });
    }


    [Authorize(Policy = nameof(ForumRulesOptions.MinimumReputationForVote))]
    [ValidateAntiForgeryToken]
    [HttpPost("{controller:slugify}/details/{id:guid}/up-vote")]
    public async Task<IActionResult> UpVoteQuestion(Guid id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId is null) return RedirectToAction("Login", "Account");

        await _votingService.QuestionUpvoteAsync(id, Guid.Parse(currentUserId));

        return RedirectToAction(nameof(Details), new { id });
    }


    [Authorize(Policy = nameof(ForumRulesOptions.MinimumReputationForVote))]
    [ValidateAntiForgeryToken]
    [HttpPost("{controller:slugify}/details/{id:guid}/down-vote")]
    public async Task<IActionResult> DownVoteQuestion(Guid id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId is null) return RedirectToAction("Login", "Account");

        await _votingService.QuestionDownVoteAsync(id, Guid.Parse(currentUserId));

        return RedirectToAction(nameof(Details), new { id });
    }


    [Authorize(Policy = nameof(ForumRulesOptions.MinimumReputationForVote))]
    [ValidateAntiForgeryToken]
    [HttpPost("{controller:slugify}/details/{questionId:guid}/answer/{answerId:guid}/up-vote")]
    public async Task<IActionResult> UpVoteAnswer(Guid questionId, Guid answerId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId is null) return RedirectToAction("Login", "Account");

        await _votingService.AnswerUpvoteAsync(answerId, Guid.Parse(currentUserId));

        return RedirectToAction(nameof(Details), new { id = questionId });
    }


    [Authorize(Policy = nameof(ForumRulesOptions.MinimumReputationForVote))]
    [ValidateAntiForgeryToken]
    [HttpPost("{controller:slugify}/details/{questionId:guid}/answer/{answerId:guid}/down-vote")]
    public async Task<IActionResult> DownVoteAnswer(Guid questionId, Guid answerId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId is null) return RedirectToAction("Login", "Account");

        await _votingService.AnswerDownVoteAsync(answerId, Guid.Parse(currentUserId));

        return RedirectToAction(nameof(Details), new { id = questionId });
    }


    [HttpGet("{controller:slugify}/details/{id:guid}/answer")]
    [Authorize]
    public IActionResult Answer(Guid id)
    {
        var viewModel = _scope.Resolve<AnswerViewModel>();
        viewModel.Id = id;
        return View("Answer", viewModel);
    }


    [HttpPost("{controller:slugify}/details/{id:guid}/answer")]
    [ValidateAntiForgeryToken, Authorize]
    public async Task<IActionResult> Answer(AnswerViewModel viewModel)
    {
        if (viewModel.Id == default)
        {
            return RedirectToAction(nameof(Index));
        }

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (currentUserId is null)
        {
            return RedirectToAction("Login", "Account");
        }

        if (ModelState.IsValid is false)
        {
            return View(viewModel);
        }

        var parsedUserId = Guid.Parse(currentUserId);

        viewModel.Resolve(_scope);

        var result = await viewModel.IsQuestionExistsAsync(parsedUserId, viewModel.Id);

        if (result is false)
        {
            RedirectToAction(nameof(Index));
        }

        await viewModel.ConductAnswerCreationAsync(viewModel.Id, parsedUserId);

        return RedirectToAction(nameof(Details), new { id = viewModel.Id });
    }


    [HttpGet("{controller:slugify}/details/{id:guid}/comment")]
    [Authorize]
    public IActionResult QuestionComment(Guid id)
    {
        var viewModel = _scope.Resolve<QuestionCommentViewModel>();
        viewModel.Id = id;
        return View(viewModel);
    }

    [HttpPost("{controller:slugify}/details/{id:guid}/comment")]
    [Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> QuestionComment(QuestionCommentViewModel viewModel)
    {
        if (viewModel.Id == default)
        {
            return RedirectToAction(nameof(Index));
        }

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId is null)
        {
            return RedirectToAction("Login", "Account");
        }

        if (ModelState.IsValid is false)
        {
            return View(viewModel);
        }

        viewModel.Resolve(_scope);
        await viewModel.ConductCommentCreateAsync(viewModel.Id, Guid.Parse(currentUserId));
        return RedirectToAction(nameof(Details), new { id = viewModel.Id });
    }


    [HttpGet("{controller:slugify}/details/{questionId:guid}/answer/{answerId:guid}/comment")]
    [Authorize]
    public IActionResult AnswerComment(Guid questionId, Guid answerId)
    {
        var viewModel = _scope.Resolve<AnswerCommentViewModel>();
        viewModel.QuestionId = questionId;
        viewModel.AnswerId = answerId;
        return View("AnswerComment", viewModel);
    }

    [HttpPost("{controller:slugify}/details/{questionId:guid}/answer/{answerId:guid}/comment")]
    [Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> AnswerComment(AnswerCommentViewModel viewModel)
    {
        if (viewModel.QuestionId == default || viewModel.AnswerId == default)
        {
            return RedirectToAction(nameof(Index));
        }

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId is null)
        {
            return RedirectToAction("Login", "Account");
        }

        if (ModelState.IsValid is false)
        {
            return View(viewModel);
        }

        viewModel.Resolve(_scope);
        await viewModel.ConductCommentCreateAsync(viewModel.QuestionId,
            viewModel.AnswerId, Guid.Parse(currentUserId));

        return RedirectToAction(nameof(Details), new { id = viewModel.QuestionId });
    }


    [HttpPost("{controller:slugify}/details/{questionId:guid}/answer/{answerId:guid}/accept")]
    [Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> AcceptAnswer(Guid questionId, Guid answerId)
    {
        var result = await _answerService.MarkAcceptedAsync(questionId, answerId);

        return RedirectToAction(nameof(Details), new { id = questionId });
    }


    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Search(string text)
    {
        var viewModel = _scope.Resolve<QuestionSearchViewModel>();

        viewModel.SearchText = text;

        return RedirectToAction(nameof(Index), viewModel);
    }
}