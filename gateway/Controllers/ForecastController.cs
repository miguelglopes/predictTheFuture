using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Net.Mime;
using gateway.Model;
using System.Threading.Tasks;
using System;


namespace gateway.Controllers
{
    /// <summary>
    /// Forecast api endpoint
    /// </summary>
    [ApiController] //  isto é inherited?
    [Route("api/forecast")]
    public class ForecastController : OSControllerBase{

        [HttpPost]
        //[Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[Produces(MediaTypeNames.Application.Json)]

        public async Task<IActionResult> ForecastModel([FromBody] ForecastRequest request)
        {
            string messageId = Guid.NewGuid().ToString("N");
            r.publishMessage(request.serialize(), RK_REQUESTFORECAST, messageId); //TODO meter em config
            return Accepted(messageId);
        }


    }
}
