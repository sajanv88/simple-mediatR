using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SimpleMediatR;

/// <summary>
///     A simple implementation of the MediatR pattern.
/// </summary>
/// <param name="serviceProvider"></param>
/// <param name="logger"></param>
public sealed class MediatR(IServiceProvider serviceProvider, ILogger<MediatR> logger) : IMediatR
{
    private static readonly ConcurrentDictionary<Type, Type> HandlerTypes = new();
    
    /// <summary>
    ///      Sends a request to the appropriate handler and returns the response.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TResponse"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Sending request of type {RequestType}", request.GetType().Name);
        cancellationToken.ThrowIfCancellationRequested();

        var requestType = request.GetType();
        if (!HandlerTypes.TryGetValue(requestType, out var handlerType))
        {
            handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));
            HandlerTypes.TryAdd(requestType, handlerType);
        }

        var handler = serviceProvider.GetService(handlerType);
        if (handler == null)
        {
            throw new InvalidOperationException($"Handler for '{requestType.Name}' not found.");
        }

        return await (Task<TResponse>) handlerType
            .GetMethod("HandleAsync")?
            .Invoke(handler, [request, cancellationToken])!;
    }

    /// <summary>
    ///     Publishes a notification to all registered notification handlers.
    /// </summary>
    /// <param name="notification"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TNotification"></typeparam>
    public async Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
    {
        cancellationToken.ThrowIfCancellationRequested();
        var handlers = serviceProvider.GetServices<INotificationHandler<TNotification>>().ToList();
        if (handlers.Count == 0) return;
        var tasks = handlers.Select(handler => handler.HandleAsync(notification, cancellationToken));
        await Task.WhenAll(tasks);
    }
}

