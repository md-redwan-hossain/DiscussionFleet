using System.Security.Claims;
using Autofac;
using DiscussionFleet.Application.AnswerFeatures;
using DiscussionFleet.Application.Common.Options;
using DiscussionFleet.Application.VotingFeatures;
using DiscussionFleet.Web.Models.AnswerWithRelated;
using DiscussionFleet.Web.Models.QuestionWithRelated;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscussionFleet.Web.Controllers;

public class QuestionsController : Controller
{
    private readonly ILifetimeScope _scope;
    private readonly IVotingService _votingService;

    public QuestionsController(ILifetimeScope scope, IVotingService votingService)
    {
        _scope = scope;
        _votingService = votingService;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] QuestionSearchViewModel viewModel)
    {
        viewModel.Resolve(_scope);
        await viewModel.FetchPostsAsync();
        return View(viewModel);
    }


    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var viewModel = _scope.Resolve<QuestionDetailsViewModel>();
        var question = await viewModel.FetchQuestionAsync(id);
        if (question is null) return NotFound();

        var author = await viewModel.FetchAuthorAsync(question.AuthorId);
        if (author is null) return NotFound();

        var result = await viewModel.FetchQuestionRelatedDataAsync(question, author);
        if (result is false) NotFound();

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var canVoteResult = await viewModel.CheckVotingAbilityAsync(currentUserId, author);
        if (canVoteResult) viewModel.CanVote = true;

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


    [HttpPost, ValidateAntiForgeryToken, Authorize]
    public async Task<IActionResult> Answer([FromRoute] Guid id, [FromForm] AnswerViewModel viewModel)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (currentUserId is null)
        {
            return RedirectToAction("Login", "Account");
        }

        // if (ModelState.IsValid is false)
        // {
        //     return View(viewModel);
        // }
        var parsedUserId = Guid.Parse(currentUserId);

        viewModel.Resolve(_scope);

        var result = await viewModel.CheckValidAuthorAsync(parsedUserId, id);

        if (result is AnswerCreateValidityResult.HomogenousUser)
        {
            ModelState.AddModelError(string.Empty, "Question asker can't answer can't answer in own question");
        }

        if (result is AnswerCreateValidityResult.QuestionNotFound)
        {
            RedirectToAction(nameof(Index));
        }

        await viewModel.ConductAnswerCreationAsync(parsedUserId);

        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize(Policy = nameof(ForumRulesOptions.MinimumReputationForVote))]
    [ValidateAntiForgeryToken]
    [HttpPost("{controller}/details/{id:guid}/up-vote")]
    public async Task<IActionResult> UpVote(Guid id)
    {
        await _votingService.QuestionUpvoteAsync(id);

        return RedirectToAction(nameof(Details), new { id });
    }


    [Authorize(Policy = nameof(ForumRulesOptions.MinimumReputationForVote))]
    [ValidateAntiForgeryToken]
    [HttpPost("{controller}/details/{id:guid}/down-vote")]
    public async Task<IActionResult> DownVote(Guid id)
    {
        await _votingService.QuestionDownVoteAsync(id);

        return RedirectToAction(nameof(Details), new { id });
    }


    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Search(string text)
    {
        Console.WriteLine();
        // if (ModelState.IsValid)
        // {
        //     return View(model);
        // }

        return RedirectToAction(nameof(Index));
    }
}