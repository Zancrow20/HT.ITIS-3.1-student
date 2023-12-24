using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Infrastructure.Validation.RequestTypes;
using Dotnet.Homeworks.Mediator;

namespace Dotnet.Homeworks.Features.Users.Decorators;

public class PermissionCheckDecorator<TRequest, TResponse> : 
    IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IPermissionCheck<IClientRequest> _permissionCheck;

    protected PermissionCheckDecorator(IPermissionCheck<IClientRequest> permissionCheck)
    {
        _permissionCheck = permissionCheck;
    }

    public virtual async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var result = await _permissionCheck.CheckPermissionAsync((request as IClientRequest)!);
        
        if (result.IsSuccess)
            return true as dynamic;

        return (result.Error as dynamic)!;
    }
}