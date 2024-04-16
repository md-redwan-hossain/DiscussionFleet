using Autofac;
using Autofac.Extensions.DependencyInjection;
using DiscussionFleet.Application;
using DiscussionFleet.Application.Common.Options;
using DiscussionFleet.EmailWorker;
using DiscussionFleet.Infrastructure;
using DiscussionFleet.Infrastructure.Extensions;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;


var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", false)
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateBootstrapLogger();


try
{
    var builder = Host.CreateApplicationBuilder(args);

    if (builder.Configuration.IsRunningInContainer())
    {
        builder.Configuration.AddEnvironmentVariables();
    }
    else
    {
        builder.Configuration.AddJsonFile("secrets.json", optional: false);
    }


    builder.Services.BindAndValidateOptions<AppSecretOptions>(AppSecretOptions.SectionName);
    builder.Services.BindAndValidateOptions<SmtpOptions>(SmtpOptions.SectionName);
    builder.Services.BindAndValidateOptions<AwsCredentialOptions>(AwsCredentialOptions.SectionName);
    builder.Services.BindAndValidateOptions<CloudQueueOptions>(CloudQueueOptions.SectionName);

    var connectionString = builder.Configuration
        .GetSection(AppSecretOptions.SectionName)
        .GetValue<string>(nameof(AppSecretOptions.DatabaseUrl));

    var sqlServerOpts = builder.Configuration
        .GetSection(SqlServerSerilogOptions.SectionName)
        .Get<SqlServerSerilogOptions>();

    ArgumentNullException.ThrowIfNull(sqlServerOpts, nameof(SqlServerSerilogOptions));

    builder.Services.AddSerilog((_, lc) => lc
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .MinimumLevel.Information()
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(configuration)
        .WriteTo.MSSqlServer(
            connectionString: connectionString,
            restrictedToMinimumLevel: Enum.Parse<LogEventLevel>(sqlServerOpts.MinimumLogLevel),
            sinkOptions: new MSSqlServerSinkOptions
            {
                TableName = sqlServerOpts.TableName,
                SchemaName = sqlServerOpts.SchemaName,
                AutoCreateSqlTable = sqlServerOpts.AutoCreateSqlTable
            }
        )
    );

    builder.Services.AddAwsConfig(builder.Configuration);
    builder.Services.AddRedisHangfireConfig(builder.Configuration);
    builder.Services.AddHostedService<Worker>();

    builder.ConfigureContainer(new AutofacServiceProviderFactory(), b =>
    {
        b.RegisterModule(new ApplicationModule());
        b.RegisterModule(new InfrastructureModule());
    });


    var host = builder.Build();
    await host.RunAsync();

    return 0;
}
catch (Exception e)
{
    Log.Fatal(e, "Host terminated unexpectedly");
    return 1;
}
finally
{
    await Log.CloseAndFlushAsync();
}