using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Infrastructure.Validation.RequestTypes;
using Dotnet.Homeworks.Mediator;
using FluentValidation;

namespace Dotnet.Homeworks.Features.Users.Decorators;

public class PermissionCheckDecorator<TRequest, TResponse> : 
    ValidationDecorator<TRequest, TResponse> 
    where TRequest : IRequest<TResponse>
{
    private readonly IPermissionCheck<IClientRequest> _permissionCheck;

    protected PermissionCheckDecorator(IPermissionCheck<IClientRequest> permissionCheck,
        IEnumerable<IValidator<TRequest>> validators) : base(validators)
    {
        _permissionCheck = permissionCheck;
    }

    public override async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        if (request is not IClientRequest clientRequest)
            return await base.Handle(request, cancellationToken);
            
        var result = await _permissionCheck.CheckPermissionAsync(clientRequest);
        
        if (result.IsSuccess)
            return await base.Handle(request, cancellationToken);

        return (result.Error as dynamic)!;
    }
}