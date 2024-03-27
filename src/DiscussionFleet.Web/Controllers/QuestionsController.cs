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
        // public IActionResult Index(int page = 1, int limit = 15)
        // {
        //     var pager = new Paginator(totalItems: 200, dataPerPage: limit, currentPage: page);
        //     return View(pager);
        // }


        ViewData["Title"] = "Questions";
        var model = _scope.Resolve<QuestionSearchViewModel>();
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Index(QuestionSearchViewModel model)
    {
        Console.WriteLine();
        if (ModelState.IsValid)
        {
            return View(model);
            // return RedirectToAction(nameof(Index));
        }

        return RedirectToAction(nameof(Index));
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