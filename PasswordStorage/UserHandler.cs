using System.Text.Json;
using Libraries;
using ApiGateway.Events;

namespace PasswordStorage;

public class UserHandler : BaseEventHandler<UserEvent> 
{
    protected override Task Handle(UserEvent result)
    {
        var message = JsonSerializer.Serialize(result);
        Console.WriteLine("TestMethod2 [x] Received {0}", message);
        return Task.FromResult(Task.CompletedTask);
    }
}