using RabbitMQ.Client;
using System.Text;

// connect cloudAmqp
ConnectionFactory factory = new();
factory.Uri = new("amqps://hqnqjkbe:w9vUNnpPxRcF3o8N11VXEuz43qVpXx-y@sparrow.rmq.cloudamqp.com/hqnqjkbe");

// create connection
using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();

// create queue
channel.QueueDeclare(queue: "example-queue", exclusive: false); // exclusive = many queue can subscribe

// RabbitMq accepts queues of type byte
byte[] message = Encoding.UTF8.GetBytes("Merhaba");
channel.BasicPublish(exchange: "", routingKey: "example-queue", basicProperties: null, message); // exchange: "" default exchange(direct)

Console.ReadLine();