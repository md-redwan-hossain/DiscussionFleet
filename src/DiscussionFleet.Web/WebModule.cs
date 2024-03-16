using Autofac;
using DiscussionFleet.Web.Models;

namespace DiscussionFleet.Web;

public class WebModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<RegistrationModel>().AsSelf();
    }
}