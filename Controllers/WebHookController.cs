using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using webhookshell.Interfaces;
using webhookshell.Models;

namespace webhookshell.Controllers
{
    [ApiController]
    [Route("[controller]/v1/")]
    public class WebHookController: ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IScriptRunner _scriptRunner;

        public WebHookController(IConfiguration config, IScriptRunner scriptRunner)
        {
            _config = config;
            _scriptRunner = scriptRunner;
        }

        [HttpGet]
        public IActionResult StartPsScript([FromQuery]DTOScript scriptFromQuery)
        {
            string key = _config.GetValue<string>("key");
            if(!string.Equals(key, scriptFromQuery.key))
            {
                return Unauthorized(new DTOResult{
                    isCompletedSuccesfully= false,
                    message = "Invalid key. Please provide a valid key to make a webhook."
                });
            }

            string stdout = _scriptRunner.Run(scriptFromQuery);
            
            return Ok(new DTOResult{
                isCompletedSuccesfully = true,
                scriptName = scriptFromQuery.script,
                param = scriptFromQuery.param,
                output = stdout
            });
        }
    }
}