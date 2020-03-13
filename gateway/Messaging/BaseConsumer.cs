using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Hosting;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Gateway.Model;
using System;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

//TODO DOCUMENTATION

namespace Gateway.Messaging
{
    public abstract class BaseConsumer : BackgroundService
    {
        private readonly MRabbitMQ connection;
        protected MessageRepository messagesRepository;
        protected IConfiguration configuration;
        protected readonly ILogger logger;


        protected abstract string Queue { get; }


        public BaseConsumer(MRabbitMQ connection, MessageRepository messagesRepository, IConfiguration configuration, ILogger logger)
        {
            this.configuration = configuration;
            this.connection = connection;
            this.messagesRepository = messagesRepository;
            this.logger = logger;
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            connection.StartConsuming(Queue, OnMessageReceived);

            return Task.CompletedTask;
        }

        public async Task OnMessageReceived(object sender, BasicDeliverEventArgs eventArgs)
        {
            await Task.Run(() => { MessageReceived(sender, eventArgs); });
        }

        protected void MessageReceived(object sender, BasicDeliverEventArgs eventArgs)
        {
            AsyncEventingBasicConsumer cons = (AsyncEventingBasicConsumer)sender;
            IModel channel = cons.Model;
            try
            {
                //get messageid
                string messageID = eventArgs.BasicProperties.MessageId;

                //get associated task and notify reception
                messagesRepository.TryRemove(messageID, out TaskCompletionSource<JSendMessage> task);
                if (task != null)
                {
                    string message = Encoding.UTF8.GetString(eventArgs.Body);
                    JSendMessage jm = JsonConvert.DeserializeObject<JSendMessage>(message);
                    /*
                     * TODO if it's not jsend, this will throw an exception and it will nack the message, even if it's the right message id.
                     * Maybe it should cancel the request immediatly? Or return fail t othe requester?
                     * I'll ignore it for now since backend always sends jsend
                    */
                    jm.MessageID = messageID;
                    task.SetResult(jm);
                }
                else
                {
                    //request expired/never existed
                    throw new Exception("Request Expired. Message will be nacked");
                }
                channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
                logger.LogInformation("Successfully processed message {}", messageID);

            }
            catch
            {
                channel.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: false);
                logger.LogWarning("Invalid message received. Message will be nacked and sent to dead letter.");
            }

        }
    }
}
