using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace webhookshell.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ErrorController: ControllerBase
    {
        public IActionResult HandleExceptions()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            return Problem(title: context.Error.Message, statusCode: 500);
        }
    }
}