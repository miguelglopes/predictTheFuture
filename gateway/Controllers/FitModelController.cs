using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Net.Mime;
using gateway.Model;
using System.Threading.Tasks;

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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[Produces(MediaTypeNames.Application.Json)]

        public async Task<FitRequest> FitModel([FromBody] FitRequest request){
            if (request.id == 0)
            {
                long i = 0;
                while (i <= 9999999999) { i = i + 1; }
            }
            r.publishMessage(request.serialize(), "request.fit"); //TODO meter em config
            return request;
        }

    }
}
