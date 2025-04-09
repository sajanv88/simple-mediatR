using Microsoft.Extensions.DependencyInjection;

namespace SimpleMediatR.Extensions;

/// <summary>
///   Provides extension methods for setting up MediatR in an ASP.NET Core application.
/// </summary>
public static class SimpleMediatrExtensions
{
    /// <summary>
    ///     Adds MediatR services to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        services.AddScoped<IMediatR, MediatR>();
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        var handlerInterfaceType = typeof(IRequestHandler<,>);
        var notificationInterfaceType = typeof(INotificationHandler<>);
        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes()
                .Where(t => t is { IsClass: true, IsAbstract: false } && t.GetInterfaces().Any());
            foreach (var type in types)
            {
                var interfaces = type.GetInterfaces();

                foreach (var i in interfaces)
                {
                    if (i.IsGenericType)
                    {
                        var genericDef = i.GetGenericTypeDefinition();

                        if (genericDef == handlerInterfaceType || genericDef == notificationInterfaceType)
                        {
                            services.AddTransient(i, type);
                        }
                    }
                }
            }
        }
        return services;
    }
}
