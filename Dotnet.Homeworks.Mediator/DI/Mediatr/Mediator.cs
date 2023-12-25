using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
namespace Dotnet.Homeworks.Mediator.DI.Mediatr;

public partial class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;
    private static readonly ConcurrentDictionary<Type, Type?> RequestHandlers = new();
    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request)); 
        var requestType = request.GetType();
        
        var handlerType = RequestHandlers.GetOrAdd(requestType, reqType =>
        {
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(reqType, typeof(TResponse));
            return handlerType;
        });
        
        var handler = _serviceProvider.GetService(handlerType!)!;
        
        if (handler == null)
            throw new ArgumentNullException($"There is no any handler registered for {requestType.Name}");

        var handleMethod = handler.GetType().GetMethod("Handle");
        
       if (!handleMethod.GetParameters().Any(p => p.ParameterType == requestType))
            throw new InvalidOperationException($"Handle method has not parameter," +
                                                $" which type equals to {requestType.Name}");

        var handlerDelegate = new RequestHandlerDelegate<TResponse>(Delegate);

        var pipelineDelegate = ConfigurePipelineBehaviors(handlerDelegate, (dynamic) request, cancellationToken);
        return await pipelineDelegate.Invoke();
        
        async Task<TResponse> Delegate() 
            => await (handler as dynamic).Handle((dynamic) request, cancellationToken)!;
    }

    public async Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var requestType = request.GetType();
        
        var handlerType = RequestHandlers.GetOrAdd(requestType, reqType =>
        {
            var handlerType = typeof(IRequestHandler<>).MakeGenericType(reqType);
            return handlerType;
        });
        
        var handler = _serviceProvider.GetService(handlerType!)!;

        if (handler is null)
            throw new ArgumentNullException($"There is no any handler registered for {requestType.Name}");
        
        var handleMethod = handler.GetType().GetMethod("Handle");
        
        if (!handleMethod.GetParameters().Any(p => p.ParameterType == requestType))
            throw new InvalidOperationException($"Handle method has not parameter," +
                                                $" which type equals to {requestType.Name}");
        
        await (handler as dynamic).Handle((dynamic) request, cancellationToken);
    }

    public async Task<dynamic?> Send(object? request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request)); 
        var requestType = request.GetType();
        var genericInterfaceType = requestType
            .GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>));

        if (genericInterfaceType is not null)
        {
            var sendMethod = typeof(Mediator)
                .GetMethods()
                .First(m => m is {Name: nameof(Send), IsGenericMethod: true, IsPublic: true} 
                            && m.ReturnParameter.GetType().IsGenericType);
            sendMethod = sendMethod.MakeGenericMethod(genericInterfaceType);
            dynamic result = sendMethod.Invoke(this, new dynamic[] {request, cancellationToken})!;
            return await result.ConfigureAwait(false);
        }
        
        var interfaceType = requestType
            .GetInterfaces()
            .FirstOrDefault(i => i == typeof(IRequest));
        if (interfaceType is not null)
        {
            return await Send((IRequest) request, cancellationToken)
                .ContinueWith(_ => Task.FromResult<dynamic?>(null), cancellationToken).Unwrap();
        }
       
        throw new ArgumentException($"{requestType.Name} doesn't implement {nameof(IRequest)}", nameof(request));
    }

    private RequestHandlerDelegate<TResponse> ConfigurePipelineBehaviors<TRequest, TResponse>(
        RequestHandlerDelegate<TResponse> handlerDelegate, TRequest request,
        CancellationToken cancellationToken)
    {
        var pipelineBehaviors = _serviceProvider
            .GetServices<IPipelineBehavior<TRequest,TResponse>>()
            .Reverse()
            .ToList();

        if (!pipelineBehaviors.Any())
            return handlerDelegate;

        RequestHandlerDelegate<TResponse> result = default!;

        foreach (var behavior in pipelineBehaviors)
        {
            if (result is null)
                result = async () => await behavior.Handle(request, handlerDelegate, cancellationToken)!;
            else
            {
                var result1 = result;
                result = async () => await behavior.Handle(request, result1, cancellationToken)!;
            }
        }

        return result;
    }

    private IPipelineBehavior<TRequest, TResponse> ConvertObjectToPipelineBehavior<TRequest, TResponse>(object input)
        => (IPipelineBehavior<TRequest, TResponse>) input;
}