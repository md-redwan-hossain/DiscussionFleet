using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DiscussionFleet.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        return services;
    }
}