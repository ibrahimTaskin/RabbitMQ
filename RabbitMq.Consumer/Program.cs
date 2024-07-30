using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

ConnectionFactory factory = new();
factory.Uri = new Uri("amqps://hqnqjkbe:w9vUNnpPxRcF3o8N11VXEuz43qVpXx-y@sparrow.rmq.cloudamqp.com/hqnqjkbe");

using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();

channel.QueueDeclare("example-queue", exclusive: false);

EventingBasicConsumer consumer = new(model: channel);
channel.BasicConsume(queue: "example-queue", autoAck: false, consumer: consumer);
consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
{
    Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
};

Console.ReadLine();