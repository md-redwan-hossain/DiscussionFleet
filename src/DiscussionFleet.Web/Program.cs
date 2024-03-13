using DiscussionFleet.Application.Common.Options;
using DiscussionFleet.Infrastructure.Extensions;
using DiscussionFleet.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

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

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/home/error");
    // The default HSTS value is 30 days.
    // You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app
    .UseStaticFiles()
    .UseRouting()
    .UseAuthentication()
    .UseAuthorization()
    .UseSession();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();