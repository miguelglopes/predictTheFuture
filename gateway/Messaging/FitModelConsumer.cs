using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


//TODO DOCUMENTATION


namespace Gateway.Messaging
{
    public class FitModelConsumer : BaseConsumer
    {
        public FitModelConsumer(MRabbitMQ connection, MessageRepository messagesRepository, IConfiguration configuration, ILogger<FitModelConsumer> logger) :
            base(connection, messagesRepository, configuration, logger)
        {
        }
        protected override string Queue => configuration.GetSection("RABBITMQ_QUEUE_RESPONSEFITS").Value;


    }
}
