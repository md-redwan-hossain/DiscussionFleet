using System.Security.Claims;
using Autofac;
using DiscussionFleet.Application.Common.Options;
using DiscussionFleet.Application.VotingFeatures;
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

    public QuestionsController(ILifetimeScope scope, IVotingService votingService)
    {
        _scope = scope;
        _votingService = votingService;
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

        var canVoteResult = await _votingService
            .CheckVotingAbilityAsync(currentUserId, author.Id, question.Id);

        viewModel.CanUpvote = canVoteResult.upvote;
        viewModel.CanDownVote = canVoteResult.downVote;

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
    [HttpPost("{controller}/details/{id:guid}/up-vote")]
    public async Task<IActionResult> UpVote(Guid id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId is null) return RedirectToAction("Login", "Account");

        await _votingService.QuestionUpvoteAsync(id, Guid.Parse(currentUserId));

        return RedirectToAction(nameof(Details), new { id });
    }


    [Authorize(Policy = nameof(ForumRulesOptions.MinimumReputationForVote))]
    [ValidateAntiForgeryToken]
    [HttpPost("{controller}/details/{id:guid}/down-vote")]
    public async Task<IActionResult> DownVote(Guid id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId is null) return RedirectToAction("Login", "Account");

        await _votingService.QuestionDownVoteAsync(id, Guid.Parse(currentUserId));

        return RedirectToAction(nameof(Details), new { id });
    }


    [HttpGet("{controller}/details/{id:guid}/answer")]
    [Authorize]
    public IActionResult Answer(Guid id)
    {
        var viewModel = _scope.Resolve<QuestionCommentViewModel>();
        viewModel.Id = id;
        return View("Comment", viewModel);
    }


    [HttpPost("{controller}/details/{id:guid}/answer")]
    [ValidateAntiForgeryToken, Authorize]
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

        var result = await viewModel.IsQuestionExistsAsync(parsedUserId, id);

        if (result is false)
        {
            RedirectToAction(nameof(Index));
        }

        await viewModel.ConductAnswerCreationAsync(parsedUserId);

        return RedirectToAction(nameof(Details), new { id });
    }


    // [HttpPost("{controller}/details/{id:guid}/comment")]
    // [Authorize, ValidateAntiForgeryToken]
    // public async Task<IActionResult> QuestionComment(QuestionCommentViewModel viewModel)
    // {
    //     if (viewModel.Id == default)
    //     {
    //         return RedirectToAction(nameof(Index));
    //     }
    //
    //     var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    //     if (currentUserId is null)
    //     {
    //         return RedirectToAction("Login", "Account");
    //     }
    //
    //     if (ModelState.IsValid is false)
    //     {
    //         return View("Comment", viewModel);
    //     }
    //
    //     viewModel.Resolve(_scope);
    //     await viewModel.ConductCommentCreateAsync(viewModel.Id, Guid.Parse(currentUserId));
    //     return RedirectToAction(nameof(Details), new { id = viewModel.Id });
    // }


    [HttpGet("{controller}/details/{id:guid}/comment")]
    [Authorize]
    public IActionResult QuestionComment(Guid id)
    {
        var viewModel = _scope.Resolve<QuestionCommentViewModel>();
        viewModel.Id = id;
        return View("Comment", viewModel);
    }

    [HttpPost("{controller}/details/{id:guid}/comment")]
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
            return View("Comment", viewModel);
        }

        viewModel.Resolve(_scope);
        await viewModel.ConductCommentCreateAsync(viewModel.Id, Guid.Parse(currentUserId));
        return RedirectToAction(nameof(Details), new { id = viewModel.Id });
    }


    // [HttpGet("{controller}/details/{id:guid}/comment")]
    // public IActionResult AnswerComment(Guid id)
    // {
    //     var viewModel = _scope.Resolve<CreateCommentViewModel>();
    //
    //     return View("Comment", viewModel);
    // }
    //
    //
    // [HttpPost, ValidateAntiForgeryToken]
    // public IActionResult AnswerComment(Guid id, CreateCommentViewModel viewModel)
    // {
    //     Console.WriteLine();
    //     // if (ModelState.IsValid)
    //     // {
    //     //     return View(model);
    //     // }
    //
    //     return RedirectToAction(nameof(Index));
    // }


    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Search(string text)
    {
        var viewModel = _scope.Resolve<QuestionSearchViewModel>();

        viewModel.SearchText = text;

        return RedirectToAction(nameof(Index), viewModel);
    }
}