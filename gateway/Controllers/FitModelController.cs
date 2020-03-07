using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Net.Mime;

namespace gateway.Controllers
{
    /// <summary>
    /// Fit_model api endpoint
    /// </summary>
    [ApiController] //  isto é inherited?
    [Route("api/fit_model")]
    [Produces(MediaTypeNames.Application.Json)]
    public class FitModelController : OSControllerBase{
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        private ActionResult<string> FitModel(){
            return "fitted model";
        }

    }
}
