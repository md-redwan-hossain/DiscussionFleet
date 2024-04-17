using Autofac;
using DiscussionFleet.Application.AnswerFeatures;
using DiscussionFleet.Application.MemberReputationFeatures;
using DiscussionFleet.Application.QuestionFeatures.Services;
using DiscussionFleet.Application.TagFeatures;

namespace DiscussionFleet.Application;

public class ApplicationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<TagService>()
            .As<ITagService>()
            .InstancePerLifetimeScope();

        builder.RegisterType<QuestionService>()
            .As<IQuestionService>()
            .InstancePerLifetimeScope();

        builder.RegisterType<AnswerService>()
            .As<IAnswerService>()
            .InstancePerLifetimeScope();

        builder.RegisterType<MemberReputationService>()
            .As<IMemberReputationService>()
            .InstancePerLifetimeScope();
    }
}