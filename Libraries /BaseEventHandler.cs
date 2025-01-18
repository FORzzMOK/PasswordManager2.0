using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;


namespace Libraries;

public class BaseEventHandler<T> where T : Event
{
    public async Task StartHandle()
    {
        var factory = new ConnectionFactory() { HostName = "localhost", UserName = "user", Password = "password" };
        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();
        await channel.QueueDeclareAsync(queue: "MyQueue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var result = Serializer<T>.Deserialize(body);
            return Handle(result);
        };

        await channel.BasicConsumeAsync(queue: "MyQueue",
            autoAck: true,
            consumer: consumer);

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }

    protected virtual Task Handle(T result)
    {
        var message = JsonSerializer.Serialize(result);
        Console.WriteLine(" [x] Received {0}", message);
        return Task.FromResult(Task.CompletedTask);
    }
}

public static class Serializer<T> where T : class
{
    public static T? Deserialize(byte[] data) 
    {
        var message = Encoding.UTF8.GetString(data);
        var result = JsonSerializer.Deserialize<T>(message);
        return result;
    }
}