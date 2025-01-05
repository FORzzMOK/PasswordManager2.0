using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace PasswordManager2._0.RabbitMq;

public interface IRabbitMqService
{
    Task SendMessage(object obj);
    Task SendMessage(string message);
}

public class RabbitMqService : IRabbitMqService
{
    public async Task SendMessage(object obj)
    {
        var message = JsonSerializer.Serialize(obj);
        await SendMessage(message);
    }

    public async Task SendMessage(string message)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();
        await channel.QueueDeclareAsync(queue: "MyQueue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var body = Encoding.UTF8.GetBytes(message);
            
        await channel.BasicPublishAsync<BasicProperties>(exchange: string.Empty,
            routingKey: "MyQueue",
            basicProperties: null,
            body: body,
            mandatory: true);
    }
}