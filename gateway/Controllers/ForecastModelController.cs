using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using Gateway.Messaging;
using Gateway.Model;
using System.Net.Mime;
using System.Text;

namespace Gateway.Controllers
{
    [ApiController]
    [Route("api/forecast")]
    public class ForecastModelController :  MigControllerBase
    {

        public ForecastModelController(MessagesRepository messagesRepository, RabbitConnection rabbitConnection)
        {
            _repository = messagesRepository;
            _connection = rabbitConnection;
            _channel = _connection.CreateChannel();
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> ForecastModel([FromBody] ForecastRequest request)
        {
            //Set message id
            string messageId = Guid.NewGuid().ToString("N");
            IBasicProperties props = _channel.CreateBasicProperties();
            props.MessageId = messageId;

            //publish request
            byte[] body = Encoding.UTF8.GetBytes(request.serialize());
            _channel.BasicPublish("predictFuture", "request.forecast", basicProperties: props, body: body);

            //wait for consumer
            TaskCompletionSource<ResponseMessage> taskCompletition = new TaskCompletionSource<ResponseMessage>();
            Task<ResponseMessage> task = taskCompletition.Task;
            _repository[messageId]=taskCompletition;
            ResponseMessage m = await task;

            return Ok(m.message);
        }

    }
}
