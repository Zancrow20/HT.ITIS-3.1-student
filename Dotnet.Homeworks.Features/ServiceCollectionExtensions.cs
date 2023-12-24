using Dotnet.Homeworks.Features.Helpers;
using Dotnet.Homeworks.Features.UserManagement.Behaviors;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker.DependencyInjectionExtensions;
using Dotnet.Homeworks.Mediator;
using Dotnet.Homeworks.Mediator.DependencyInjectionExtensions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnet.Homeworks.Features;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFeaturesServices(this IServiceCollection services)
    {   
        services.AddMediator(AssemblyReference.Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PermissionPipelineBehavior<,>));

        services.AddPermissionChecks(AssemblyReference.Assembly);

        services.AddValidatorsFromAssembly(AssemblyReference.Assembly);
        return services;
    }
}