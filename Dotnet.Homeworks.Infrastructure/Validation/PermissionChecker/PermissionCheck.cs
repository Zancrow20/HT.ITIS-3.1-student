/*using Dotnet.Homeworks.Infrastructure.Utils;
using Dotnet.Homeworks.Mediator;

namespace Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;

public class PermissionCheck : IPermissionCheck
{
    private readonly IServiceProvider _serviceProvider;

    public PermissionCheck(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<IEnumerable<PermissionResult>> CheckPermissionAsync<TRequest>(TRequest request)
    {
        var checkTypes = request
            .GetType()
            .GetInterfaces()
            .Where(IsNotCqrsInterface)
            .Distinct()
            .ToList();

        var permissionChecks = _serviceProvider
            .GetServices(typeof(IPermissionCheck<>))
            .Select(ch => (IPermissionCheck<TRequest>)ch!);
        
        var checkers = permissionChecks
            .Where(p => checkTypes.Contains(p.GetType().GetGenericArguments()[0]));

        var results = checkers
            .Select(async ch => await ch.CheckPermissionAsync(request));
        return await Task.WhenAll(results);
    }

    private static bool IsNotCqrsInterface(Type type)
        => type != typeof(IRequest) && type != typeof(IRequest<>);
}*/
