namespace Libraries;

public interface IRabbitMqService<in T> where T : Event
{
    Task SendMessage(T obj);
}