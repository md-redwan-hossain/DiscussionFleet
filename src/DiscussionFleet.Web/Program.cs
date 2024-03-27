using Autofac;
using Autofac.Extensions.DependencyInjection;
using DiscussionFleet.Application;
using DiscussionFleet.Application.Common.Options;
using DiscussionFleet.Infrastructure;
using DiscussionFleet.Infrastructure.Extensions;
using DiscussionFleet.Infrastructure.Persistence;
using DiscussionFleet.Infrastructure.Utils;
using DiscussionFleet.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.BindAndValidateOptions<AppOptions>(AppOptions.SectionName);
builder.Services.BindAndValidateOptions<JwtOptions>(JwtOptions.SectionName);

var dbUrl = builder
    .Configuration.GetSection(AppOptions.SectionName)
    .GetValue<string>("DatabaseUrl");

builder.Services.AddDbContext<ApplicationDbContext>
    (opts => opts.UseSqlServer(dbUrl));

builder.Services.AddJwtAuth(builder.Configuration);
builder.Services.AddIdentityConfiguration();
builder.Services.AddCookieAuthentication();

builder.Services.Configure<RouteOptions>(options =>
{
    options.ConstraintMap["slugify"] = typeof(SlugifyParameterTransformer);
    options.AppendTrailingSlash = false;
});


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


app.MapRazorPages();

app.Run();