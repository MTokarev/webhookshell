using System;
using System.Collections.Generic;
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
            .FirstOrDefault(script => string.Equals(script.Name, scriptToCheck.Script, StringComparison.InvariantCultureIgnoreCase));
        
        if (scriptMapping?.Trigger?.HttpMethods is null)
        {
            return result;
        }
        
        if (!HttpTriggerMethod.TryParse(httpContext.Request.Method, out HttpTriggerMethod httpMethod))
        {
            result.Errors.Add($"Unable to parse HTTP Method '{httpContext.Request.Method}' to available methods '{string.Join(", ", scriptMapping.Trigger.HttpMethods)}'.");
            return result;
        }
        
        if (scriptMapping.Trigger.HttpMethods.Contains(httpMethod))
        {
            return result;
        } 

        result.Errors.Add($"Unable to launch the script '{scriptMapping?.Name}'. Please make sure that you use the right HTTP Method. " + 
            $"Allowed methods: '{string.Join(", ", scriptMapping.Trigger.HttpMethods)}'.");

        return result;
    }
  }
}