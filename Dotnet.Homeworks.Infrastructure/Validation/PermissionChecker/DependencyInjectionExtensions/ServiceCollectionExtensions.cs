using System.Reflection;

namespace Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker.DependencyInjectionExtensions;

public static class ServiceCollectionExtensions
{
    public static void AddPermissionChecks(
        this IServiceCollection serviceCollection,
        Assembly assembly)
    {
        RegisterCheckersFromAssembly(serviceCollection, assembly);
    }
    
    public static void AddPermissionChecks(
        this IServiceCollection serviceCollection,
        IEnumerable<Assembly> assemblies)
    {
        RegisterCheckersFromAssemblies(serviceCollection, assemblies);
    }
    
    private static void RegisterCheckersFromAssembly(IServiceCollection services,Assembly assembly)
    {
        var scanResults = GetCheckersFromAssembly(assembly);
        foreach (var service in scanResults)
        {
            var permissionType = service
                .GetInterfaces()
                .First(i => i.GetGenericTypeDefinition() == typeof(IPermissionCheck<>))
                .GetGenericArguments()[0];
            
            services.AddTransient(typeof(IPermissionCheck<>)
                .MakeGenericType(permissionType), service);
        }
    }

    private static void RegisterCheckersFromAssemblies(IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        foreach (var assembly in assemblies)
            RegisterCheckersFromAssembly(services, assembly);
    }

    private static IEnumerable<Type> GetCheckersFromAssembly(Assembly assembly)
    {
        var checkers = assembly
            .GetTypes()
            .Where(t => t
                .GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPermissionCheck<>)));
        return checkers;
    }
}