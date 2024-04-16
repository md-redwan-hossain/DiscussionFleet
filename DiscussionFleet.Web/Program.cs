using Autofac;
using Autofac.Extensions.DependencyInjection;
using DiscussionFleet.Application;
using DiscussionFleet.Application.Common.Options;
using DiscussionFleet.Infrastructure;
using DiscussionFleet.Infrastructure.Extensions;
using DiscussionFleet.Infrastructure.Utils;
using DiscussionFleet.Web;
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
    var builder = WebApplication.CreateBuilder(args);

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
    builder.Services.BindAndValidateOptions<ForumRulesOptions>(ForumRulesOptions.SectionName);
    builder.Services.BindAndValidateOptions<AwsCredentialOptions>(AwsCredentialOptions.SectionName);
    builder.Services.BindAndValidateOptions<FileBucketOptions>(FileBucketOptions.SectionName);
    builder.Services.BindAndValidateOptions<CloudQueueOptions>(CloudQueueOptions.SectionName);
    builder.Services.BindAndValidateOptions<SqlServerSerilogOptions>(SqlServerSerilogOptions.SectionName);


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
    
    await builder.Services.AddDatabaseConfigAsync(builder.Configuration);
    builder.Services.AddRedisConfig(builder.Configuration);

    builder.Services.AddIdentityConfiguration();
    builder.Services.AddCookieAuthentication();
    builder.Services.Configure<RouteOptions>(options =>
    {
        options.ConstraintMap["slugify"] = typeof(SlugifyParameterTransformer);
        options.AppendTrailingSlash = false;
    });

    builder.Services.AddAwsConfig(builder.Configuration);
    builder.Services.AddControllersWithViews();


    builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
    builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
    {
        containerBuilder.RegisterModule(new ApplicationModule());
        containerBuilder.RegisterModule(new InfrastructureModule());
        containerBuilder.RegisterModule(new WebModule());
    });


    var app = builder.Build();

    app.UseStatusCodePagesWithReExecute("/home/error/{0}");

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/home/error");
        app.UseHsts();
    }


    app
        .UseStaticFiles()
        .UseRouting()
        .UseAuthentication()
        .UseAuthorization();
// .UseSession();

    app.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists:slugify}/{controller:slugify=Home}/{action:slugify=Index}/{id?}");


    app.MapControllerRoute(
        name: "default",
        pattern: "{controller:slugify=Home}/{action:slugify=Index}/{id?}");

    await app.RunAsync();
    return 0;
}


catch (HostAbortedException)
{
    Log.Information("HostAbortedException handled");
    return 0;
}

catch (Exception e)
{
    Console.WriteLine(e);
    return 1;
}
finally
{
    await Log.CloseAndFlushAsync();
}