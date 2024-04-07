using Autofac;
using DiscussionFleet.Web.Models;

namespace DiscussionFleet.Web;

public class WebModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<QuestionSearchViewModel>().AsSelf();

        // Account related
        builder.RegisterType<RegistrationViewModel>().AsSelf();
        builder.RegisterType<ConfirmAccountViewModel>().AsSelf();
        builder.RegisterType<LoginViewModel>().AsSelf();
    }
}