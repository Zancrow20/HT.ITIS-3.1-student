using Dotnet.Homeworks.Features.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnet.Homeworks.Features;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMediatr(this IServiceCollection services)
    {
        return services.AddMediatR(cfg 
            => cfg.RegisterServicesFromAssembly(AssemblyReference.Assembly));
    }
}