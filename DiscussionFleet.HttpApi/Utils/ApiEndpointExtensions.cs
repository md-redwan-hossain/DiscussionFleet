using System.Reflection;

namespace DiscussionFleet.HttpApi.Utils;

public static class ApiEndpointExtensions
{
    public static IEndpointRouteBuilder MapApiEndpointsFromAssembly(this IEndpointRouteBuilder endpoints,
        Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(endpoints);
        ArgumentNullException.ThrowIfNull(assembly);


        var typeOfIApiEndpoint = typeof(IApiEndpoint);

        var implementorsOfIApiEndpoint = assembly.GetTypes().Where(t =>
            t is { IsClass: true, IsAbstract: false, IsGenericType: false }
            && typeOfIApiEndpoint.IsAssignableFrom(t));

        foreach (var item in implementorsOfIApiEndpoint)
        {
            var mapEndpointsMethod = item.GetMethod(
                nameof(IApiEndpoint.DefineRoutes),
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            
            mapEndpointsMethod?.Invoke(null, [endpoints]);
        }


        return endpoints;
    }
}