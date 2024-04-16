using Autofac;
using DiscussionFleet.Web.Models;
using DiscussionFleet.Web.Models.Account;
using DiscussionFleet.Web.Models.QuestionWithRelated;

namespace DiscussionFleet.Web;

public class WebModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<QuestionSearchViewModel>().AsSelf();

        // Account related
        builder.RegisterType<ResendVerificationCodeViewModel>().AsSelf();
        builder.RegisterType<ConfirmAccountViewModel>().AsSelf();
        builder.RegisterType<RegistrationViewModel>().AsSelf();
        builder.RegisterType<LoginViewModel>().AsSelf();
        builder.RegisterType<ProfileViewModel>().AsSelf();

        // Question related
        builder.RegisterType<QuestionAskViewModel>().AsSelf();
        builder.RegisterType<QuestionDetailsViewModel>().AsSelf();
    }
}