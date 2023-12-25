using Dotnet.Homeworks.Features.Helpers;
using Dotnet.Homeworks.Features.UserManagement.Behaviors;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker.DependencyInjectionExtensions;
using Dotnet.Homeworks.Mediator;
using Dotnet.Homeworks.Mediator.DependencyInjectionExtensions;
using FluentValidation;

namespace Dotnet.Homeworks.MainProject.ServicesExtensions.СQRSValidation;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFeaturesServices(this IServiceCollection services)
    {
        services.AddMediator(AssemblyReference.Assembly)
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(PermissionPipelineBehavior<,>))
            .AddValidatorsFromAssembly(AssemblyReference.Assembly)
            .AddPermissionChecks(AssemblyReference.Assembly);
        return services;
    }
}