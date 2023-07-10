using Microsoft.AspNetCore.Mvc;
using Webhookshell.Interfaces;
using Webhookshell.Models;

namespace Webhookshell.Controllers
{
    [ApiController]
    [Route("[controller]/v1/")]
    public class WebHookController : ControllerBase
    {
        private readonly IScriptRunnerService _scriptRunner;

        public WebHookController(IScriptRunnerService scriptRunner)
        {
            _scriptRunner = scriptRunner;
        }

        [HttpGet]
        public IActionResult StartScriptFromGet([FromQuery] DtoScript scriptFromQuery)
        {
            Result<DtoResult> scriptRun = _scriptRunner.Run(scriptFromQuery, HttpContext);

            if (scriptRun.IsValid)
            {
                return Ok(scriptRun.Data);
            }

            return BadRequest(scriptRun.Errors);
        }

        [HttpPost]
        public IActionResult StartScriptFromPost([FromBody] DtoScript scriptFromBody)
        {
            Result<DtoResult> scriptRun = _scriptRunner.Run(scriptFromBody, HttpContext);

            if (scriptRun.IsValid)
            {
                return Ok(scriptRun.Data);
            }

            return BadRequest(scriptRun.Errors);
        }
    }
}