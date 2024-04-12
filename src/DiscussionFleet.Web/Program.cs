using Autofac;
using Autofac.Extensions.DependencyInjection;
using DiscussionFleet.Application;
using DiscussionFleet.Application.Common.Options;
using DiscussionFleet.Infrastructure;
using DiscussionFleet.Infrastructure.Extensions;
using DiscussionFleet.Infrastructure.Utils;
using DiscussionFleet.Web;
using DiscussionFleet.Web.Utils;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("secrets.json");

builder.Services.BindAndValidateOptions<AppSecretOptions>(AppSecretOptions.SectionName);
builder.Services.BindAndValidateOptions<JwtOptions>(JwtOptions.SectionName);
builder.Services.BindAndValidateOptions<SmtpOptions>(SmtpOptions.SectionName);
builder.Services.BindAndValidateOptions<AwsCredentialOptions>(AwsCredentialOptions.SectionName);
builder.Services.BindAndValidateOptions<FileBucketOptions>(FileBucketOptions.SectionName);

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
    // app.UseStatusCodePagesWithReExecute("/home/error/{0}");
    // The default HSTS value is 30 days.
    // You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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

app.Run();