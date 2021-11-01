using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PlatformService.DTOs;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration configuration;
        private IConnection connection;
        private readonly IModel channel;

        public MessageBusClient(IConfiguration configuration)
        {
            this.configuration = configuration;
            var factory = new ConnectionFactory() { HostName = configuration["RabbitMQHost"], Port = int.Parse(configuration["RabbitMQPort"]) };

            try
            {
                connection = factory.CreateConnection();
                channel = connection.CreateModel();

                channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

                connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"=== Could not connect to Message Bus {ex.Message}");
            }
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine($"=== RabbitMQ connection shut down");
        }

        void IMessageBusClient.PublishNewPlatform(PlatformPublishDto platformPublishDto)
        {
            var message = JsonSerializer.Serialize(platformPublishDto);

            if (connection.IsOpen)
            {
                Console.WriteLine($"=== RabbitMQ connection open, sending message");
                SendMessage(message);
            }
            else
            {
                Console.WriteLine($"=== RabbitMQ connection closed, can't send message");
            }
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "trigger", routingKey: "", basicProperties: null, body: body);
            Console.WriteLine($"=== RabbitMQ send message {message}");
        }

        public void Dispose()
        {
            Console.WriteLine($"=== RabbitMQ disposed");

            if (channel.IsOpen)
            {
                channel.Close();
                connection.Close();
            }
        }
    }
}