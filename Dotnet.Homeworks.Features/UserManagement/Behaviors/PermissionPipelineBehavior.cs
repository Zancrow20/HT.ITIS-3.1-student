using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Infrastructure.Validation.RequestTypes;
using Dotnet.Homeworks.Mediator;
using Dotnet.Homeworks.Shared.Dto;

namespace Dotnet.Homeworks.Features.UserManagement.Behaviors;

public class PermissionPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : IRequest<TResponse>, IAdminRequest
    where TResponse : Result
{
    private readonly IPermissionCheck<IAdminRequest> _permissionCheck;

    public PermissionPipelineBehavior(IPermissionCheck<IAdminRequest> permissionCheck)
    {
        _permissionCheck = permissionCheck;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var permissionResult = await _permissionCheck.CheckPermissionAsync(request);
        if (permissionResult.IsFailure)
            return (permissionResult.Error as dynamic)!;

        return await next.Invoke();
    }
}