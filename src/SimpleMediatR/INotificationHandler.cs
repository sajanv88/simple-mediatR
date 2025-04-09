namespace SimpleMediatR;

/// <summary>
///    Represents a handler for processing notifications in the MediatR pattern.
/// </summary>
/// <typeparam name="TNotification"></typeparam>
public interface INotificationHandler<in TNotification> where TNotification : INotification
{
    Task HandleAsync(TNotification notification, CancellationToken cancellationToken);
}
