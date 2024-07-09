using Autofac;
using DiscussionFleet.Application.AnswerFeatures;
using DiscussionFleet.Application.QuestionFeatures.Services;
using DiscussionFleet.Application.ResourceNotificationFeatures;
using DiscussionFleet.Application.TagFeatures;
using DiscussionFleet.Application.VotingFeatures;

namespace DiscussionFleet.Application;

public class ApplicationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<ResourceNotificationService>()
            .As<IResourceNotificationService>()
            .InstancePerLifetimeScope();
        
        builder.RegisterType<TagService>()
            .As<ITagService>()
            .InstancePerLifetimeScope();

        builder.RegisterType<QuestionService>()
            .As<IQuestionService>()
            .InstancePerLifetimeScope();

        builder.RegisterType<AnswerService>()
            .As<IAnswerService>()
            .InstancePerLifetimeScope();

        builder.RegisterType<VotingService>()
            .As<IVotingService>()
            .InstancePerLifetimeScope();
    }
}