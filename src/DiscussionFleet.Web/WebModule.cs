using Autofac;
using DiscussionFleet.Web.Models;

namespace DiscussionFleet.Web;

public class WebModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<RegistrationViewModel>().AsSelf();
        builder.RegisterType<QuestionSearchViewModel>().AsSelf();
        builder.RegisterType<ConfirmAccountViewModel>().AsSelf();
    }
}