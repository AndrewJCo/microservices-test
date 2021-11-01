using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommandService.EventProcessing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandService.AsyncDataServices
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IConfiguration configuration;
        private readonly IEventProcessor eventProcessor;
        private IConnection connection;
        private IModel channel;
        private string queueName;

        public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
        {
            this.configuration = configuration;
            this.eventProcessor = eventProcessor;

            InitialiseRabbitMQ();
        }

        private void InitialiseRabbitMQ()
        {
            var factory = new ConnectionFactory()
            {
                HostName = configuration["RabbitMQHost"],
                Port = int.Parse(configuration["RabbitMQPort"])
            };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
            queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName,
            exchange: "trigger",
            routingKey: "");

            Console.WriteLine("=== Listening on the message bus");

            connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("=== Connection shutdown");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("=== Getting commands for platform");

            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (ModuleHandle, ea) => {
                Console.WriteLine("=== Event received");

                var body = ea.Body;
                var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

                eventProcessor.ProcessEvent(notificationMessage);
            };

            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            if (channel.IsOpen)
            {
                channel.Close();
                connection.Close();
            }

            base.Dispose();
        }
    }
}