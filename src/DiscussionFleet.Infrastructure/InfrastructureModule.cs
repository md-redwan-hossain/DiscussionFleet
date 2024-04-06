using Autofac;
using DiscussionFleet.Application;
using DiscussionFleet.Application.Common.Providers;
using DiscussionFleet.Application.Common.Services;
using DiscussionFleet.Application.MembershipFeatures;
using DiscussionFleet.Domain.Repositories;
using DiscussionFleet.Infrastructure.Email;
using DiscussionFleet.Infrastructure.Identity.Services;
using DiscussionFleet.Infrastructure.Persistence;
using DiscussionFleet.Infrastructure.Persistence.Repositories;
using DiscussionFleet.Infrastructure.Providers;

namespace DiscussionFleet.Infrastructure;

public class InfrastructureModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<JwtProvider>()
            .As<IJwtProvider>()
            .SingleInstance();

        builder.RegisterType<DateTimeProvider>()
            .As<IDateTimeProvider>()
            .SingleInstance();

        builder.RegisterType<GuidProvider>()
            .As<IGuidProvider>()
            .SingleInstance();


        builder.RegisterType<QrCodeProvider>()
            .As<IQrCodeProvider>()
            .SingleInstance();


        builder.Register(c => c.Resolve<ApplicationDbContext>())
            .As<IApplicationDbContext>()
            .InstancePerLifetimeScope();


        builder.RegisterType<AnswerRepository>()
            .As<IAnswerRepository>()
            .InstancePerLifetimeScope();

        builder.RegisterType<BadgeRepository>()
            .As<IBadgeRepository>()
            .InstancePerLifetimeScope();

        builder.RegisterType<BlogCategoryRepository>()
            .As<IBlogCategoryRepository>()
            .InstancePerLifetimeScope();

        builder.RegisterType<BlogPostRepository>()
            .As<IBlogPostRepository>()
            .InstancePerLifetimeScope();

        builder.RegisterType<ForumRuleRepository>()
            .As<IForumRuleRepository>()
            .InstancePerLifetimeScope();

        builder.RegisterType<MemberRepository>()
            .As<IMemberRepository>()
            .InstancePerLifetimeScope();

        builder.RegisterType<MultimediaImageRepository>()
            .As<IMultimediaImageRepository>()
            .InstancePerLifetimeScope();

        builder.RegisterType<QuestionRepository>()
            .As<IQuestionRepository>()
            .InstancePerLifetimeScope();

        builder.RegisterType<TagRepository>()
            .As<ITagRepository>()
            .InstancePerLifetimeScope();

        builder.RegisterType<ResourceNotificationRepository>()
            .As<IResourceNotificationRepository>()
            .InstancePerLifetimeScope();

        builder.RegisterType<ApplicationUnitOfWork>()
            .As<IApplicationUnitOfWork>()
            .InstancePerLifetimeScope();

        builder.RegisterType<MemberService>()
            .As<IMemberService>()
            .InstancePerLifetimeScope();

        builder.RegisterType<HtmlEmailService>()
            .As<IEmailService>()
            .InstancePerLifetimeScope();

        builder.RegisterType<JsonSerializationProvider>()
            .As<IJsonSerializationProvider>()
            .SingleInstance();
    }
}