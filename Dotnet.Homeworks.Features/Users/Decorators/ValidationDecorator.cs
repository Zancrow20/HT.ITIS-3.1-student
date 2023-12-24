using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Infrastructure.Validation.RequestTypes;
using Dotnet.Homeworks.Mediator;
using FluentValidation;

namespace Dotnet.Homeworks.Features.Users.Decorators;

public class ValidationDecorator<TRequest, TResponse> : 
    PermissionCheckDecorator<TRequest, TResponse> 
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    
    protected ValidationDecorator(IPermissionCheck<IClientRequest> permissionCheck,
        IEnumerable<IValidator<TRequest>> validators) : base(permissionCheck)
    {
        _validators = validators;
    }


    public override async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        if(!_validators.Any()) 
            return await base.Handle(request, cancellationToken);

        var results = await Task.WhenAll(_validators
            .Select(v => v.ValidateAsync(request, cancellationToken)));

        var errors = results
            .SelectMany(r => r.Errors)
            .Select(e => e.ErrorMessage);

        if (!errors.Any())
            return true as dynamic;
        
        return errors.Aggregate((acc, next) => $"{acc} {next}")! as dynamic;
    }
}