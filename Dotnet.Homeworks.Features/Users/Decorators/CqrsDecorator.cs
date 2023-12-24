using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Infrastructure.Validation.RequestTypes;
using Dotnet.Homeworks.Mediator;
using FluentValidation;

namespace Dotnet.Homeworks.Features.Users.Decorators;

public class CqrsDecorator<TRequest, TResponse> : 
    ValidationDecorator<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    protected CqrsDecorator(IPermissionCheck<IClientRequest> permissionCheck,
        IEnumerable<IValidator<TRequest>> validators) : base(permissionCheck, validators)
    { }
}