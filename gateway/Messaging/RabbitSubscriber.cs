using System;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Hosting;
using System.Threading;
using Gateway.Model;
using System.Text;

namespace Gateway.Messaging
{
    public class RabbitSubscriber : BackgroundService
    {
        private readonly RabbitConnection _connection;
        private IModel _channel;
        private string _queue = "forecastsQueue";

        private MessagesRepository _messagesRepository;


        public RabbitSubscriber(RabbitConnection connection, MessagesRepository messagesRepository)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _messagesRepository = messagesRepository;
            InitChannel();
        }

        private void InitChannel()
        {
          
            _channel = _connection.CreateChannel();

            _channel.CallbackException += (sender, ea) =>
            {
                InitChannel();
            };
        }


         protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += OnMessageReceived;
            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            _channel.BasicConsume(queue: _queue, autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }

        private void OnMessageReceived(object sender, BasicDeliverEventArgs eventArgs)
        {
            string messsageID = eventArgs.BasicProperties.MessageId;
            ResponseMessage rm = new ResponseMessage();
            rm.message = Encoding.UTF8.GetString(eventArgs.Body);
            rm.messageID = messsageID;
            _messagesRepository[messsageID].SetResult(rm);
            //TODO REMOVE KEY?
        }

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { }
        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }
        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }
    }
}
