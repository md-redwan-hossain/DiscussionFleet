using Autofac;

namespace DiscussionFleet.Web.Utils;

public interface IViewModelWithResolve
{
    void Resolve(ILifetimeScope scope);
}