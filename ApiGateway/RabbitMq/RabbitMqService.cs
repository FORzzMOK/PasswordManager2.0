using System.Text;
using System.Text.Json;
using ApiGateway.Models;
using RabbitMQ.Client;

namespace ApiGateway.RabbitMq;

public interface IRabbitMqService
{
    Task SendMessage(User user);
}

public class RabbitMqService : IRabbitMqService
{
    public async Task SendMessage(User user)
    {
        var factory = new ConnectionFactory() { HostName = "localhost", UserName = "user", Password = "password" };
        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();
        await channel.QueueDeclareAsync(queue: "MyQueue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var result = Serialize<User>(user);
        
        await channel.BasicPublishAsync<BasicProperties>(exchange: string.Empty,
            routingKey: "MyQueue",
            basicProperties: new BasicProperties(),
            body: result,
            mandatory: true);
        
    }
    
    private static byte[] Serialize<T>(T obj) where T : class
    {
        var message = JsonSerializer.Serialize(obj);
        var result = Encoding.UTF8.GetBytes(message);
        return result;
    }
}