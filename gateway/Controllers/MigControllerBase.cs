using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using Gateway.Messaging;

namespace Gateway.Controllers
{
    /// <summary>
    /// The base Controller class.
    /// Contains common methods for a controller.
    /// </summary>
    [ApiController]
    public class MigControllerBase : ControllerBase
    {
        protected MessagesRepository _repository;
        protected RabbitConnection _connection;
        protected IModel _channel;


    }
}
