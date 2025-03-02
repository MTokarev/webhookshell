using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Webhookshell.Controllers
{
    [ApiController]
    [Route("[controller]")]
    // This controller is not documented in the Swagger UI
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        [HttpGet]
        public IActionResult HandleExceptions()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            return Problem(title: context.Error.Message, statusCode: 500);
        }
    }
}