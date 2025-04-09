// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimpleMediatR;
using SimpleMediatR.Extensions;

var services = new ServiceCollection();
services.AddLogging(builder => builder.AddConsole());
services.AddMediatr();


var provider = services.BuildServiceProvider();
var mediatR = provider.GetRequiredService<IMediatR>();
var logger = provider.GetRequiredService<ILogger<Program>>();


var ping = new Ping { Message = "Hello, World!" };
var result = await mediatR.SendAsync(ping);

logger.LogInformation($"Received response  ({result})");

var userCreationDto = new UserCreationDto("John", "Doe", Guid.NewGuid());
var isSuccess = await mediatR.SendAsync(new NewUserCreationRequest { User = userCreationDto });

var status = isSuccess ? "success" : "failure";
logger.LogInformation($"User creation: {status}");




record UserCreationDto(string FirstName, string LastName, Guid Id);

class NewUserCreationRequest : IRequest<bool>
{
    public UserCreationDto User { get; set; }
}

class NewUserCreationHandler(ILogger<NewUserCreationHandler> logger, IMediatR mediatR) : IRequestHandler<NewUserCreationRequest, bool>
{
    public async Task<bool> HandleAsync(NewUserCreationRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Creating user: {request.User.FirstName} {request.User.LastName} ({request.User.Id})");

        // Notify NewUserNotificationHandler about the new user creation
        await mediatR.PublishAsync(new NewUserCreatedEvent
        {
            FirstName = request.User.FirstName,
            LastName = request.User.LastName,
            Id = request.User.Id
        }, cancellationToken);
        
       return await Task.FromResult(true);
    }
}


// Request example
class Ping : IRequest<string>
{
    public string Message { get; set; } = string.Empty;
}

class PingHandler : IRequestHandler<Ping, string>
{
    public Task<string> HandleAsync(Ping request, CancellationToken cancellationToken)
    {
        return Task.FromResult($"Pong: {request.Message}");
    }
}

// Notification example
class NewUserCreatedEvent : INotification
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Guid Id { get; set; }
}

class NewUserNotificationHandler(ILogger<NewUserNotificationHandler> logger) : INotificationHandler<NewUserCreatedEvent>
{
    public async Task HandleAsync(NewUserCreatedEvent newUser, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Received notification: FirstName: {newUser.FirstName} LastName: {newUser.LastName} UserId: {newUser.Id}");
        await Task.CompletedTask;
    }
}