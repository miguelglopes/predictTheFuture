using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


//TODO DOCUMENTATION


namespace Gateway.Messaging
{

    public class MRabbitMQ
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        protected readonly ILogger<MRabbitMQ> logger;


        public MRabbitMQ(IConfiguration configuration, ILogger<MRabbitMQ> logger)
        {
            _connectionFactory = new ConnectionFactory();
            _connectionFactory.Uri = new Uri(configuration.GetSection("RABBITMQ_URL").Value);
            _connectionFactory.DispatchConsumersAsync = true;
            _connection = _connectionFactory.CreateConnection();
            this.logger = logger;
            logger.LogInformation("Successfully connected to RabbitMQ");
        }

        public IModel CreateChannel()
        {
            return _connection.CreateModel();
        }

        internal void StartConsuming(string queue, AsyncEventHandler<BasicDeliverEventArgs> OnMessageReceived)
        {
            IModel channel = CreateChannel();
            AsyncEventingBasicConsumer consumer = new AsyncEventingBasicConsumer(channel);

            consumer.Received += OnMessageReceived;

            channel.BasicConsume(queue: queue, autoAck: false, consumer: consumer);
            logger.LogInformation("Started consuming from queue {}", queue);
        }

    }
}
