using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Webhookshell.Interfaces;
using Webhookshell.Models;
using Webhookshell.Options;

namespace Webhookshell.Validators
{
  public class HttpTriggerValidator : IScriptValidator
  {
    private readonly ScriptOptions _options;

    public HttpTriggerValidator(IOptionsSnapshot<ScriptOptions> options)
    {
        _options = options.Value;
    }
    public Result<DtoResult> Validate(DtoScript scriptToCheck, ScriptHandler handler, HttpContext httpContext = null)
    {
        Result<DtoResult> result = new();
       
        var scriptMapping = handler.ScriptsMapping
            .Where(script => string.Equals(script.Name, scriptToCheck.Script, StringComparison.InvariantCultureIgnoreCase))
            .FirstOrDefault();

        if (scriptMapping?.Trigger is null)
        {
            return result;
        }

        if (scriptMapping.Trigger.HttpMethod is not null)
        {
            if (!string.Equals(scriptMapping.Trigger.HttpMethod.ToString(), httpContext.Request.Method, StringComparison.CurrentCultureIgnoreCase))
            {
                result.Errors.Add($"Unable to launch the script '{scriptMapping?.Name}'. Please make sure that you use the right HTTP Method.");
            }
        }

        return result;
    }
  }
}