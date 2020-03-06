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
        private readonly IPswhService _pswhService;

        public WebHookController(IConfiguration config, IPswhService pswhService)
        {
            _config = config;
            _pswhService = pswhService;
        }

        [HttpGet]
        public IActionResult StartPsScript([FromQuery]DTOPswh pswh)
        {
            string key = _config.GetValue<string>("key");
            if(!string.Equals(key, pswh.key))
            {
                return Unauthorized("Invalid key. Please provide a valid key to make a webhook.");
            }

            _pswhService.RunPswh(pswh);
            return Ok($"Script '{pswh.script}' has been succesfully executed.");
        }
    }
}