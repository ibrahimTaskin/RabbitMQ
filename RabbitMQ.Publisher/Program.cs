using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;

var builder = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory) // Use AppContext.BaseDirectory instead of Directory.GetCurrentDirectory()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

IConfiguration configuration = builder.Build();

ConnectionFactory factory = new();
factory.Uri = new Uri(configuration["RabbitMq"]);
// create connection
using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();

// create queue
// exclusive = many queue can subscribe
// durable = kalıcı kuyruk, yine de iletiler kaybolabilir. (outbox, inbox pattern uygulanabilir)
channel.QueueDeclare(queue: "example-queue", exclusive: false,durable: false); 

IBasicProperties basicProperties = channel.CreateBasicProperties();
basicProperties.Persistent = true;

for (int i = 0; i < 100; i++)
{
    // RabbitMq accepts queues of type byte
    await Task.Delay(200);
    byte[] message = Encoding.UTF8.GetBytes("Merhaba " +  i);
    channel.BasicPublish(exchange: "", routingKey: "example-queue", basicProperties: basicProperties, message);
}

Console.Read();