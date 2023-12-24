using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Dotnet.Homeworks.Mediator.DI.Mediatr;

public partial class Mediator
{
    public static void RegisterHandlersFromAssemblies(IServiceCollection serviceCollection, IEnumerable<Assembly> assemblies)
    {
        var requestHandlersTypes = assemblies
            .SelectMany(a => 
                a.GetTypes().Where(t => t.Name.Contains("Handler") 
                                        && t is {IsClass: true, IsAbstract: false} 
                                        && t.GetInterfaces().Any(IsImplementIHandlerInterface)))
            .ToList();

        var interfaces = requestHandlersTypes
            .SelectMany(x => x.GetInterfaces())
            .Where(x => x.IsGenericType)
            .Where(x => x.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
            .ToList();
        
        foreach (var requestInterfaceType in interfaces)
        {
            var requestHandlerType = requestHandlersTypes.FirstOrDefault(x => x.IsAssignableTo(requestInterfaceType));
            if (requestHandlerType == null)
            {
                Console.WriteLine($"There is no implementation for {requestInterfaceType.Name}");
                continue;
            }

            //var genericArguments = requestHandlerType.GetInterface(requestInterfaceType.Name)?.GetGenericArguments();
            //var genericRequestInterfaceType = requestInterfaceType.MakeGenericType(genericArguments![0], genericArguments[1]);
            
            serviceCollection.TryAddTransient(requestInterfaceType, requestHandlerType);
        }
    }

    private static bool IsImplementIHandlerInterface(Type requestHandlerType)
    {
        return requestHandlerType.IsGenericType && 
               (requestHandlerType.GetGenericTypeDefinition() == typeof(IRequestHandler<>) ||
               requestHandlerType.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));
    }
}