using System.Reflection;
using System.Text;
using DiscussionFleet.Application.Common.Options;
using DiscussionFleet.Application.Common.Utils;
using DiscussionFleet.Infrastructure.Identity.Managers;
using DiscussionFleet.Infrastructure.Persistence;
using Hangfire;
using Hangfire.Redis.StackExchange;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace DiscussionFleet.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection BindAndValidateOptions<TOptions>(this IServiceCollection services,
        string sectionName)
        where TOptions : class
    {
        services.AddOptions<TOptions>()
            .BindConfiguration(sectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        return services;
    }


    public static async Task<IServiceCollection> AddDatabaseConfigAsync(this IServiceCollection services,
        IConfiguration configuration)
    {
        var dbUrl = configuration.GetSection(AppSecretOptions.SectionName)
            .GetValue<string>(nameof(AppSecretOptions.DatabaseUrl));

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer(dbUrl);

        await using (var dbContext = new ApplicationDbContext(optionsBuilder.Options))
        {
            var canConnect = await dbContext.Database.CanConnectAsync();
            if (canConnect is false) throw new Exception("Database is not functional");
        }

        services.AddDbContext<ApplicationDbContext>(
            dbContextOptions => dbContextOptions
                .UseSqlServer(dbUrl)
                .UseEnumCheckConstraints()
                .LogTo(Console.WriteLine, LogLevel.Error)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
        );

        services.AddDataProtection().PersistKeysToDbContext<ApplicationDbContext>();

        return services;
    }


    public static IServiceCollection AddRedisConfig(this IServiceCollection services, IConfiguration configuration)
    {
        var distCache = configuration.GetSection(AppSecretOptions.SectionName)
            .GetValue<string>(nameof(AppSecretOptions.RedisDistributedCacheUrl));

        var stackExchange = configuration.GetSection(AppSecretOptions.SectionName)
            .GetValue<string>(nameof(AppSecretOptions.RedisStackExchangeUrl));

        ArgumentNullException.ThrowIfNull(distCache);
        ArgumentNullException.ThrowIfNull(stackExchange);

        var redis = ConnectionMultiplexer.Connect(stackExchange);

        services.AddSingleton<IConnectionMultiplexer>(redis);

        services.AddHangfire(opt =>
        {
            opt.UseRedisStorage(redis, new RedisStorageOptions { Prefix = nameof(Assembly.GetExecutingAssembly) });
        });

        services.AddHangfireServer();

        services.AddStackExchangeRedisCache(opts =>
        {
            opts.Configuration = distCache;
            opts.InstanceName = RedisConstants.StackExchangeInstance;
        });


        return services;
    }


    public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services)
    {
        const string allowedCharsInPassword =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

        // services.Configure<DataProtectionTokenProviderOptions>(options =>
        // {
        //     options.TokenLifespan = TimeSpan.FromMinutes(15);
        // });


        services.AddIdentity<ApplicationUser, ApplicationRole>(
                options =>
                {
                    options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
                    options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
                }
            )
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddUserManager<ApplicationUserManager>()
            .AddRoleManager<ApplicationRoleManager>()
            .AddSignInManager<ApplicationSignInManager>()
            .AddDefaultTokenProviders();

        services.Configure<IdentityOptions>(options =>
        {
            // Password settings
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 0;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings
            options.User.AllowedUserNameCharacters = allowedCharsInPassword;
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedAccount = true;
            // options.SignIn.RequireConfirmedEmail = true;
        });

        return services;
    }


    public static IServiceCollection AddCookieAuthentication(this IServiceCollection services)
    {
        // services.AddAuthentication(o => { o.DefaultScheme = IdentityConstants.ApplicationScheme; })
        //     .AddIdentityCookies(o =>
        //     {
        //         o.ApplicationCookie?.Configure(options =>
        //         {
        //             options.Cookie.HttpOnly = true;
        //             options.LoginPath = new PathString("/account/login");
        //             options.AccessDeniedPath = new PathString("/account/login");
        //             options.LogoutPath = new PathString("/account/logout");
        //             options.Cookie.Name = "Identity";
        //             options.SlidingExpiration = true;
        //             options.ExpireTimeSpan = TimeSpan.FromHours(1);
        //         });
        //     });
        // return services;

        services.AddAuthentication()
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.LoginPath = new PathString("/account/login");
                options.AccessDeniedPath = new PathString("/account/login");
                options.LogoutPath = new PathString("/account/logout");
                options.Cookie.Name = "Identity";
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(1);
            });

        return services;
    }

    public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

        ArgumentNullException.ThrowIfNull(jwtOptions);

        services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.Secret)
                    )
                };
            });
        return services;
    }
}