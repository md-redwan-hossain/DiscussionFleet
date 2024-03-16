using Autofac;

namespace DiscussionFleet.Web.Models;

public class RegistrationModel
{
    private ILifetimeScope _scope;

    public RegistrationModel(ILifetimeScope scope)
    {
        _scope = scope;
    }

    public void Act()
    {
    }

    public void Resolve(ILifetimeScope scope)
    {
        _scope = scope;
    }
}