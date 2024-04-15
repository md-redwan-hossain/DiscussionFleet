using Autofac;
using Autofac.Extensions.DependencyInjection;
using DiscussionFleet.Application;
using DiscussionFleet.Application.Common.Options;
using DiscussionFleet.EmailWorker;
using DiscussionFleet.Infrastructure;
using DiscussionFleet.Infrastructure.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile("secrets.json", optional: false);

builder.Services.BindAndValidateOptions<AppSecretOptions>(AppSecretOptions.SectionName);
builder.Services.BindAndValidateOptions<SmtpOptions>(SmtpOptions.SectionName);
builder.Services.BindAndValidateOptions<AwsCredentialOptions>(AwsCredentialOptions.SectionName);
builder.Services.BindAndValidateOptions<CloudQueueOptions>(CloudQueueOptions.SectionName);

builder.Services.AddAwsConfig(builder.Configuration);
builder.Services.AddRedisHangfireConfig(builder.Configuration);
builder.Services.AddHostedService<Worker>();

builder.ConfigureContainer(new AutofacServiceProviderFactory(), b =>
{
    b.RegisterModule(new ApplicationModule());
    b.RegisterModule(new InfrastructureModule());
});


var host = builder.Build();
host.Run();