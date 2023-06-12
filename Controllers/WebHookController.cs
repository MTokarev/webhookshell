using Microsoft.AspNetCore.Mvc;
using webhookshell.Interfaces;
using webhookshell.Models;

namespace webhookshell.Controllers
{
    [ApiController]
    [Route("[controller]/v1/")]
    public class WebHookController: ControllerBase
    {
        private readonly IScriptRunner _scriptRunner;

        public WebHookController(IScriptRunner scriptRunner)
        {
            _scriptRunner = scriptRunner;
        }

        [HttpGet]
        public IActionResult StartPsScript([FromQuery]DTOScript scriptFromQuery)
        {
            Result<DTOResult> scriptRun = _scriptRunner.Run(scriptFromQuery);

            if (scriptRun.IsValid)
            {
                return Ok(scriptRun.Data);
            }

            return BadRequest(scriptRun.Errors);
        }
    }
}