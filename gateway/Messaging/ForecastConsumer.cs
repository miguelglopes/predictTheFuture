using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;



//TODO DOCUMENTATION


namespace Gateway.Messaging
{
    public class ForecastConsumer : BaseConsumer
    {
        public ForecastConsumer(MRabbitMQ connection, MessageRepository messagesRepository, IConfiguration configuration, ILogger<ForecastConsumer> logger) :
            base(connection, messagesRepository, configuration, logger)
        {
        }
        protected override string Queue => configuration.GetSection("RABBITMQ_QUEUE_RESPONSEFORECASTS").Value;

    }
}
