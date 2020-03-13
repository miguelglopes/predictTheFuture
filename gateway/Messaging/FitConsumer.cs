using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


//TODO DOCUMENTATION


namespace Gateway.Messaging
{
    public class FitConsumer : BaseConsumer
    {
        public FitConsumer(MRabbitMQ connection, MessageRepository messagesRepository, IConfiguration configuration, ILogger<FitConsumer> logger) :
            base(connection, messagesRepository, configuration, logger)
        {
        }
        protected override string Queue => configuration.GetSection("RABBITMQ_QUEUE_RESPONSEFITS").Value;


    }
}
