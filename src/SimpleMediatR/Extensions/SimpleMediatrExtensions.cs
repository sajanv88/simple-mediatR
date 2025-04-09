using Microsoft.Extensions.DependencyInjection;

namespace SimpleMediatR.Extensions;

public static class SimpleMediatrExtensions
{
    public static IServiceCollection AddMediatr(this IServiceCollection services)
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
