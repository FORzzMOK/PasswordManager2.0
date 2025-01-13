using Libraries;

namespace ApiGateway.Events;

public class UserEvent : Event
{
    public required string Login { get; init; }
    
    public required string Password { get; init; }
    
    public required string Role { get; init; }
}