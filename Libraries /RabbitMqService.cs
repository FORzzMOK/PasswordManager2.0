using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace Libraries;

public class RabbitMqService<T> : IRabbitMqService<T> where T : Event
{
    public async Task SendMessage(T obj)
    {
        var factory = new ConnectionFactory() { HostName = "localhost", UserName = "user", Password = "password" };
        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();
        await channel.QueueDeclareAsync(queue: "MyQueue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var result = Serialize(obj);
        
        await channel.BasicPublishAsync<BasicProperties>(exchange: string.Empty,
            routingKey: "MyQueue",
            basicProperties: new BasicProperties(),
            body: result,
            mandatory: true);
        
    }
    
    private static byte[] Serialize(T obj) 
    {
        var message = JsonSerializer.Serialize(obj);
        var result = Encoding.UTF8.GetBytes(message);
        return result;
    }
}