using System.Security.Claims;
using Autofac;
using DiscussionFleet.Web.Models.Question;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscussionFleet.Web.Controllers;

public class QuestionsController : Controller
{
    private readonly ILifetimeScope _scope;

    public QuestionsController(ILifetimeScope scope)
    {
        _scope = scope;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] QuestionSearchViewModel viewModel)
    {
        viewModel.Resolve(_scope);
        await viewModel.FetchPostsAsync();
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
        var error = await viewModel.ConductAskQuestion(Guid.Parse(currentUserId));
        if (error is not null)
        {
            viewModel.HasError = true;
            ModelState.AddModelError(string.Empty, error);
            return View(viewModel);
        }

        return RedirectToAction(nameof(Ask));
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