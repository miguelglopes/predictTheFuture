using Microsoft.AspNetCore.Mvc;
using common.Messaging;
using System;

namespace gateway.Controllers
{

    /// <summary>
    /// The base Controller class.
    /// Contains common methods for a controller.
    /// </summary>
    [ApiController]
    public class OSControllerBase : ControllerBase
    {
        protected string RK_REQUESTFORECAST=Environment.GetEnvironmentVariable("RK_REQUESTFORECAST");
        
        protected string RK_REQUESTFIT=Environment.GetEnvironmentVariable("RK_REQUESTFIT");
        protected Rabbit r = new Rabbit();
    }
}
