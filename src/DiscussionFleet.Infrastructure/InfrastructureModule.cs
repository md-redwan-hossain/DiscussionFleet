using Autofac;
using DiscussionFleet.Application;
using DiscussionFleet.Application.Common.Providers;
using DiscussionFleet.Application.Common.Services;
using DiscussionFleet.Domain.Repositories;
using DiscussionFleet.Infrastructure.CloudFileBucket;
using DiscussionFleet.Infrastructure.Email;
using DiscussionFleet.Infrastructure.Identity.Services;
using DiscussionFleet.Infrastructure.Persistence;
using DiscussionFleet.Infrastructure.Persistence.Repositories;
using DiscussionFleet.Infrastructure.Providers;
using DiscussionFleet.Infrastructure.Services;

namespace DiscussionFleet.Infrastructure;

public class InfrastructureModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        #region Data Access

        builder.RegisterType<AnswerRepository>()
            .As<IAnswerRepository>()
            .InstancePerLifetimeScope();

        builder.RegisterType<BadgeRepository>()
            .As<IBadgeRepository>()
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

        #endregion

        #region Services

        builder.RegisterType<MemberService>()
            .As<IMemberService>()
            .InstancePerLifetimeScope();

        builder.RegisterType<HtmlEmailService>()
            .As<IEmailService>()
            .InstancePerLifetimeScope();

        builder.RegisterType<FileBucketService>()
            .As<IFileBucketService>()
            .InstancePerLifetimeScope();

        #endregion

        #region Providers

        builder.RegisterType<JsonSerializationProvider>()
            .As<IJsonSerializationProvider>()
            .SingleInstance();

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

        #endregion

        #region External Libraries

        builder.RegisterType<MarkdownService>()
            .As<IMarkdownService>()
            .SingleInstance();

        #endregion
    }
}