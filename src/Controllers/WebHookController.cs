using Microsoft.AspNetCore.Mvc;
using Webhookshell.Interfaces;
using Webhookshell.Models;

namespace Webhookshell.Controllers
{
    /// <summary>
    /// This controller is responsible for running the scripts.
    /// </summary>
    [ApiController]
    [Route("[controller]/v1/")]
    public class WebHookController : ControllerBase
    {
        private readonly IScriptRunnerService _scriptRunner;

        public WebHookController(IScriptRunnerService scriptRunner)
        {
            _scriptRunner = scriptRunner;
        }

        /// <summary>
        /// Run scripts by GET request
        /// All script parameters are passed as query parameters
        /// Use POST method if you need to pass a script in the request body
        /// </summary>
        /// <param name="scriptFromQuery"><see cref="DtoScript"/></param>
        /// <returns><see cref="Result{T}"/></returns>
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

        /// <summary>
        /// Run scripts by POST request
        /// You need to pass a script in the request body
        /// Use GET method if you need to pass a script as query parameters
        /// </summary>
        /// <param name="scriptFromBody"><see cref="DtoScript"/></param>
        /// <returns><see cref="Result{T}"/></returns>
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