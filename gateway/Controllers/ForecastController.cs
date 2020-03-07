using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Net.Mime;

namespace gateway.Controllers
{
    /// <summary>
    /// Forecast api endpoint
    /// </summary>
    [ApiController] //  isto é inherited?
    [Route("api/forecast")]
    [Produces(MediaTypeNames.Application.Json)]
    public class ForecastController : OSControllerBase{
        
        [HttpPost]
        //[ProducesResponseType(StatusCodes.Status201Created)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<string> Forecast(){
            return "forecasted";
        }

    }
}
