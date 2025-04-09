namespace SimpleMediatR;

/// <summary>
/// Represents an abstraction for sending requests and publishing notifications using the MediatR pattern.
/// </summary>
public interface IMediatR
{
    /// <summary>
    /// Sends a request through the mediator pipeline to the appropriate handler and returns a response.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response expected from the request handler.</typeparam>
    /// <param name="request">The request object to be processed.</param>
    /// <param name="cancellationToken">A cancellation token for cancelling the request processing.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the response from the handler.</returns>
    Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Publishes a notification to all registered notification handlers asynchronously.
    /// </summary>
    /// <typeparam name="TNotification">The type of the notification being published.</typeparam>
    /// <param name="notification">The notification object to publish.</param>
    /// <param name="cancellationToken">A cancellation token for cancelling the publishing process.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification;
}
