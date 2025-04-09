using Microsoft.Extensions.DependencyInjection;
using SimpleMediatR;
using SimpleMediatR.Extensions;

namespace SimpleMediatRTests;

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

class WithoutHandler : IRequest<string>
{
    public string Message { get; set; } = string.Empty;
}


public class SimpleMediatRTest
{
    [Fact(DisplayName = "Should send request and receive response")]
    public async Task Test1()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMediatR();
        
        var provider = services.BuildServiceProvider();
        var mediatR = provider.GetRequiredService<IMediatR>();
        
        var ping = new Ping { Message = "Hello, World!" };
        var result = await mediatR.SendAsync(ping);
        Assert.Equal("Pong: Hello, World!", result);
    }
    
    [Fact(DisplayName = "Should throw exception when handler not found")]
    public async Task Test2()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMediatR();
        
        var provider = services.BuildServiceProvider();
        var mediatR = provider.GetRequiredService<IMediatR>();
        
        var ping = new WithoutHandler { Message = "Hello, World!" };
        
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await mediatR.SendAsync(ping);
        });
    }
    
    
    [Fact(DisplayName = "Should throw exception when cancellation is requested")]
    public async Task Test3()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMediatR();
        
        var provider = services.BuildServiceProvider();
        var mediatR = provider.GetRequiredService<IMediatR>();
        
        var ping = new Ping { Message = "Hello, World!" };
        
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            await mediatR.SendAsync(ping, cts.Token);
        });
    }
}
