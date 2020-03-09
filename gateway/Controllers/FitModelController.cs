using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Net.Mime;
using gateway.Model;
using System.Threading.Tasks;
using System;

namespace gateway.Controllers
{
    /// <summary>
    /// Fit_model api endpoint
    /// </summary>
    [ApiController] //  isto é inherited?
    [Route("api/fit_model")]
    public class FitModelController : OSControllerBase{

        [HttpPost]
        //[Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[Produces(MediaTypeNames.Application.Json)]

        public async Task<IActionResult> FitModel([FromBody] FitRequest request){
            if(request.data==null)
                return BadRequest("data field not present");
            string messageId = Guid.NewGuid().ToString("N");
            r.publishMessage(request.serialize(), RK_REQUESTFIT, messageId); //TODO meter em config
            return Accepted(messageId);
        }

    }
}
