using System.ComponentModel.DataAnnotations;
using Autofac;
using DiscussionFleet.Web.Utils;

namespace DiscussionFleet.Web.Models;

public class QuestionAskViewModel : IViewModelWithResolve
{
    [Required] public string Title { get; set; }
    [Required] public string Body { get; set; }
    public bool HasError { get; set; }
    public HashSet<string> Tags { get; set; } = [];

    public void Resolve(ILifetimeScope scope)
    {
    }
}