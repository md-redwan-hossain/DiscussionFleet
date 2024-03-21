using Autofac;
using DiscussionFleet.Web.Models;
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
    public IActionResult Index()
    {
        ViewData["Title"] = "Questions";
        var model = _scope.Resolve<QuestionSearchViewModel>();
        return View(model);
    }


    // [HttpPost, ValidateAntiForgeryToken]
    // [HttpGet]
    // public IActionResult Index(QuestionSearchFilterViewModel model)
    // {
    //     Console.WriteLine();
    //     if (ModelState.IsValid)
    //     {
    //         return RedirectToAction(nameof(Index));
    //     }
    //
    //     return RedirectToAction(nameof(Index));
    // }
}