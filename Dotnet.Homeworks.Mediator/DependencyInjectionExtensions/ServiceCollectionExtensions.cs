using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnet.Homeworks.Mediator.DependencyInjectionExtensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMediator(this IServiceCollection services, params Assembly[] handlersAssemblies)
    {
        DI.Mediatr.Mediator.RegisterHandlersFromAssemblies(services,handlersAssemblies);
        services.AddTransient<IMediator, DI.Mediatr.Mediator>();
        return services;
    }
}