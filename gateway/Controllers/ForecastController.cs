using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using Gateway.Messaging;
using Gateway.Model;
using System.Net.Mime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

//TODO DOCUMENTATION

namespace Gateway.Controllers
{
    [ApiController]
    [Route("api/forecast")]
    public class ForecastController : MControllerBase
    {

        protected override string Exchange => configuration.GetSection("RABBITMQ_EXCHANGE").Value;
        protected override string RoutingKey => configuration.GetSection("RABBITMQ_RK_REQUESTFORECAST").Value;

        public ForecastController(MessageRepository messagesRepository, Messaging.MRabbitMQ rabbitConnection, IConfiguration configuration, ILogger<ForecastController> logger) :
            base(messagesRepository, rabbitConnection, configuration, logger)
        {

        }


        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> ForecastModel([FromBody] ForecastRequest request)
        {

            logger.LogInformation("forecast Request Received");

            //Set message id
            IBasicProperties props = SetMessageId();

            //Validation performed by data model

            //get task for consumer - insert message in repository
            Task<JSendMessage> task = AwaitConsumerTask(props.MessageId);

            //publish request
            PublishMessage(props, request.Serialize());

            //wait for consumer
            JSendMessage m = null;
            try
            {
                m = await task;
            }
            catch (TaskCanceledException e)
            {
                m = JSendMessage.GetErrorMessage("Backend didn't respond in time.");
                m.MessageID = props.MessageId;
            }

            if (m.IsSuccess())
            {
                logger.LogWarning("Successfully processed {}", m.MessageID);
                return StatusCode(StatusCodes.Status200OK, m);
            }
            else if (m.IsFail())
            {
                logger.LogWarning("Failed processing message {}.", m.MessageID);
                return StatusCode(StatusCodes.Status400BadRequest, m);
            }
            else
            {
                logger.LogWarning("Error processing message {}. {}", m.MessageID, m.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, m);
            }


        }

    }
}
