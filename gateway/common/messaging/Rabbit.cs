using RabbitMQ.Client;
using System;
using System.Text;

//TODO meter isto num package common
//TODO assure only 1 channel when needed or a new channel etc

namespace common.Messaging
{
    public class Rabbit
    {
        private const string EXCHANGE_NAME = "predictFuture"; //TODO meter num config file
        private IModel channel = null;
        private IConnection con = null;

        public Rabbit()
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "localhost";
            factory.UserName = "rabbitmq";
            factory.Password = "rabbitmq";
            this.con = factory.CreateConnection();
            this.channel = this.getNewChannel();
        }

        public IModel getNewChannel()
        {
            return this.con.CreateModel();
        }

        public void publishMessage(string message, string routingKey, IBasicProperties properties = null)
        {
            byte[] body = Encoding.UTF8.GetBytes(message);
            this.channel.BasicPublish(exchange: EXCHANGE_NAME, routingKey: routingKey, basicProperties: null, body: body);
        }
    }
}