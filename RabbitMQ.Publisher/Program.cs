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
channel.QueueDeclare(queue: "example-queue", exclusive: false); // exclusive = many queue can subscribe

// RabbitMq accepts queues of type byte
byte[] message = Encoding.UTF8.GetBytes("Merhaba");
channel.BasicPublish(exchange: "", routingKey: "example-queue", basicProperties: null, message); // exchange: "" default exchange(direct)

Console.ReadLine();