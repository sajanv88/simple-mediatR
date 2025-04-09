# SimpleMediatR
A lightweight and easy-to-use implementation of the MediatR pattern for
.NET Core applications. This library provides a clean and decoupled way to send request, 
and publish notifications within your application using the mediator pattern.

If you are looking for a simple and effective way to manage communication between components in your .NET Core application, 
SimpleMediatR is the perfect solution.

## ✨ Features

- Supports request/response and notification patterns
- Decouples business logic using the mediator design pattern
- Lightweight and minimal dependencies
- Easy to integrate into existing .NET Core projects

## 📦 Installation

```bash
dotnet add package SimpleMediatR
```

Or via NuGet Package Manager:

```powershell
Install-Package SimpleMediatR
```

🛠️ Usage
1. Register MediatR in your DI Container
2. 
```csharp
builder.Services.AddMediatr(); // Automatically registers all handlers in the assembly
```
2. Define a Request and its Handler

```csharp
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


var ping = new Ping { Message = "Hello, World!" };

// Inject `IMediatR  mediatR` then use it
var result = await mediatR.SendAsync(ping);

logger.LogInformation($"Received response  ({result})");
```

🔔 Notifications
You can also publish notifications to multiple handlers:

```csharp

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

// Publishing when a new user is created
 await mediatR.PublishAsync(new NewUserCreatedEvent
        {
            FirstName = request.User.FirstName,
            LastName = request.User.LastName,
            Id = request.User.Id
        }, cancellationToken);
```

## More examples:
Checkout the examples folder for more usage examples.


🤝 Contributing
Contributions are welcome! Feel free to open issues, suggest features, or submit pull requests.

