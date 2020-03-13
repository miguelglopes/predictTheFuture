using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using Gateway.Messaging;
using System;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Threading.Tasks;
using Gateway.Model;
using System.Threading;
using Microsoft.Extensions.Logging;

//TODO DOCUMENTATION

namespace Gateway.Controllers
{

    [ApiController]
    public abstract class MControllerBase : ControllerBase
    {

        protected MessageRepository repository;
        private readonly MRabbitMQ connection;
        protected IModel channel;
        protected IConfiguration configuration;
        protected readonly ILogger logger;

        protected abstract string Exchange { get; }
        protected abstract string RoutingKey { get; }

        protected MControllerBase(MessageRepository messagesRepository, MRabbitMQ rabbitConnection, IConfiguration configuration, ILogger logger)
        {
            repository = messagesRepository;
            connection = rabbitConnection;
            channel = connection.CreateChannel();
            this.configuration = configuration;
            this.logger = logger;
        }

        protected IBasicProperties SetMessageId()
        {
            IBasicProperties props = channel.CreateBasicProperties();
            props.MessageId = Guid.NewGuid().ToString("N");
            return props;
        }

        protected void PublishMessage(IBasicProperties props, string message)
        {
            byte[] body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(Exchange, RoutingKey, basicProperties: props, body: body);
            logger.LogInformation("Request {} sent to backend.", props.MessageId);

        }

        protected Task<JSendMessage> AwaitConsumerTask(string messageID)
        {

            TaskCompletionSource<JSendMessage> taskCompletition = new TaskCompletionSource<JSendMessage>();
            Task<JSendMessage> task = taskCompletition.Task;
            repository[messageID] = taskCompletition;

            //set task timeout
            int timeout = Int32.Parse(configuration.GetSection("ASPNETCORE_BACKEND_TIMEOUT").Value);
            CancellationTokenSource ct = new CancellationTokenSource(timeout);
            ct.Token.Register(() => taskCompletition.TrySetCanceled(), useSynchronizationContext: false);

            return task;
        }

    }
}
