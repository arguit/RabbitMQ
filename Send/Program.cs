using RabbitMQ.Client;
using System;
using System.Text;

namespace Send
{
    class Program
    {
        private const string HOST_NAME = "localhost";
        private const string QUEUE_NAME = "Test";

        static void Main(string[] args)
        {
            // create connection factory with default guest/guest credentials
            // these credntials are limited to localhost connections
            var factory = new ConnectionFactory()
            {
                HostName = HOST_NAME
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                // we must declare queue before we can use it
                // if not existing then the queue will be created
                channel.QueueDeclare(
                    queue: QUEUE_NAME,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                // create a message in a plain text and encoded
                // version that will be sent to message queue
                var message = "Hi there RabbitMQ #^_^)/";
                var body = Encoding.UTF8.GetBytes(message);

                // create basic message properties
                // can be used for example for CorrelationID propagation
                var basicProperties = channel.CreateBasicProperties();
                basicProperties.CorrelationId = Guid.NewGuid().ToString();

                // publish the message to a desired message queue
                channel.BasicPublish(
                    exchange: string.Empty,
                    routingKey: QUEUE_NAME,
                    basicProperties: basicProperties,
                    body: body);

                Console.WriteLine($"Message sent [CorrelationID:{ basicProperties.CorrelationId }]: { message }");
            }

            Console.WriteLine("Pressa any key to exit.");
            Console.ReadKey();
        }
    }
}
