using System.ComponentModel.DataAnnotations;
using Autofac;
using DiscussionFleet.Application;
using DiscussionFleet.Web.Utils;

namespace DiscussionFleet.Web.Models;

public class QuestionAskViewModel : IViewModelWithResolve
{
    private ILifetimeScope _scope;
    private IApplicationUnitOfWork _appUnitOfWork;


    public QuestionAskViewModel()
    {
    }


    public QuestionAskViewModel(ILifetimeScope scope, IApplicationUnitOfWork appUnitOfWork)
    {
        _scope = scope;
        _appUnitOfWork = appUnitOfWork;
    }

    [Required] public string Title { get; set; }
    [Required] public string Body { get; set; }
    public bool HasError { get; set; }
    public HashSet<string> Tags { get; set; } = [];


    // public Task ConductAskQuestion()
    // {
    //     
    // }
    
    public void Resolve(ILifetimeScope scope)
    {
        _scope = scope;
        _appUnitOfWork = _scope.Resolve<IApplicationUnitOfWork>();
    }
}