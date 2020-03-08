using Microsoft.AspNetCore.Mvc;
using common.Messaging;

namespace gateway.Controllers
{

    /// <summary>
    /// The base Controller class.
    /// Contains common methods for a controller.
    /// </summary>
    [ApiController]
    public class OSControllerBase : ControllerBase
    {
        protected Rabbit r = new Rabbit();
    }
}
