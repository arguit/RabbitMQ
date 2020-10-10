using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Receive
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

                // create consumer and hook-up the message received event                
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (sender, e) =>
                {
                    // display body and CorrelationID the from received message
                    var body = e.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var basicProperties = e.BasicProperties;
                    
                    Console.WriteLine($"Message received [CorrelationId:{ basicProperties.CorrelationId }]: { message }");
                };

                // register the created consumer
                channel.BasicConsume(
                    queue: QUEUE_NAME,
                    autoAck: true,
                    consumer: consumer);

                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
            }
        }
    }
}
