using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var builder = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory) // Use AppContext.BaseDirectory instead of Directory.GetCurrentDirectory()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

IConfiguration configuration = builder.Build();

ConnectionFactory factory = new();
factory.Uri = new Uri(configuration["RabbitMq"]);

using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();

channel.QueueDeclare("example-queue", exclusive: false, durable: false);

EventingBasicConsumer consumer = new(model: channel);
channel.BasicConsume(queue: "example-queue", autoAck: false, consumer: consumer);
channel.BasicQos(prefetchSize:0,prefetchCount: 1, global: false); // fair dispatch

consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
{
    Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
    channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
};

Console.ReadLine();