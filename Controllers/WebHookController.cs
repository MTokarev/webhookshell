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
        private readonly IPwshService _pwshService;

        public WebHookController(IConfiguration config, IPwshService pswhService)
        {
            _config = config;
            _pwshService = pswhService;
        }

        [HttpGet]
        public IActionResult StartPsScript([FromQuery]DTOPswh pwsh)
        {
            string key = _config.GetValue<string>("key");
            if(!string.Equals(key, pwsh.key))
            {
                return Unauthorized("Invalid key. Please provide a valid key to make a webhook.");
            }

            _pwshService.RunPswh(pwsh);
            return Ok($"Script '{pwsh.script}' has been succesfully executed.");
        }
    }
}