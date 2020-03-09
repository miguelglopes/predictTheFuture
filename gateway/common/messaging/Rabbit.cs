using RabbitMQ.Client;
using System.Collections.Generic;
using System.Text;
using System;

//TODO meter isto num package common
//TODO assure only 1 channel when needed or a new channel etc

namespace common.Messaging
{
    public class Rabbit
    {
        private string EXCHANGE_NAME = Environment.GetEnvironmentVariable("EXCHANGE"); //TODO meter num config file
        private string AMQPURL = Environment.GetEnvironmentVariable("AMQPURL");
        private IModel channel = null;
        private IConnection con = null;

        public Rabbit()
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.Uri = new Uri(AMQPURL);
            this.con = factory.CreateConnection();
            this.channel = this.getNewChannel();
        }

        public IModel getNewChannel()
        {
            return this.con.CreateModel();
        }

        public void publishMessage(string message, string routingKey, string messageId)
        {
            IBasicProperties props = this.channel.CreateBasicProperties();
            props.Headers = new Dictionary<string, object>();
            props.Headers.Add("messageId", messageId);
            byte[] body = Encoding.UTF8.GetBytes(message);
            this.channel.BasicPublish(exchange: EXCHANGE_NAME, routingKey: routingKey, basicProperties: props, body: body);
        }
    }
}