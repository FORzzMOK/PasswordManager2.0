using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory() { HostName = "localhost", UserName = "user", Password = "password" };
await using var connection = await factory.CreateConnectionAsync();
await using var channel = await connection.CreateChannelAsync();
await channel.QueueDeclareAsync(queue: "MyQueue",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);

var consumer = new AsyncEventingBasicConsumer(channel);
//var consumer = new EventingBasicConsumer(channel);
consumer.ReceivedAsync += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine(" [x] Received {0}", message);
    return Task.CompletedTask;
};

await channel.BasicConsumeAsync(queue: "MyQueue",
    autoAck: true,
    consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();
