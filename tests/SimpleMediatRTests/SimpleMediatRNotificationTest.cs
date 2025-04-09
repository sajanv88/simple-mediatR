using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using SimpleMediatR;
using SimpleMediatR.Extensions;

namespace SimpleMediatRTests;
public class NewUserCreatedEvent : INotification
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Guid Id { get; set; }
}

public class NewUserNotificationHandler(ILogger<NewUserNotificationHandler> logger)
    : INotificationHandler<NewUserCreatedEvent>
{
    public async Task HandleAsync(NewUserCreatedEvent newUser, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Received notification: FirstName: {newUser.FirstName} LastName: {newUser.LastName} UserId: {newUser.Id}");
        await Task.CompletedTask;
    }
}

public class SimpleMediatRNotificationTest
{
    [Fact(DisplayName = "Should send notification")]
    public async Task Test1()
    {
        var loggerMock = new Mock<ILogger<NewUserNotificationHandler>>();

        var services = new ServiceCollection();
        services.AddSingleton(loggerMock.Object); // 👈 register mock logger
        services.AddLogging();
        services.AddMediatR();

        var provider = services.BuildServiceProvider();
        var mediatR = provider.GetRequiredService<IMediatR>();

        var newUser = new NewUserCreatedEvent
        {
            FirstName = "John",
            LastName = "Doe",
            Id = Guid.NewGuid()
        };

        await mediatR.PublishAsync(newUser);
        
        loggerMock.Verify(
            l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) =>
                    state.ToString()!.Contains($"FirstName: {newUser.FirstName}") &&
                    state.ToString()!.Contains($"LastName: {newUser.LastName}") &&
                    state.ToString()!.Contains($"UserId: {newUser.Id}")
                ),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
        
    }
    
}
