using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Webhookshell.Interfaces;
using Webhookshell.Models;

namespace Webhookshell.Validators
{
  public class TimeValidator : IScriptValidator
  {
    public Result<DtoResult> Validate(DtoScript scriptToCheck, ScriptHandler handler, HttpContext httpContext = null)
    {
      Result<DtoResult> result = new();

        var scriptMapping = handler.ScriptsMapping
            .Where(script => string.Equals(script.Name, scriptToCheck.Script, StringComparison.InvariantCultureIgnoreCase))
            .FirstOrDefault();

        if (scriptMapping.Trigger?.TimeFrame?.StartUtc is null 
            || scriptMapping.Trigger.TimeFrame?.EndUtc is null)
        {
            return result;
        }

        TimeSpan startUtc = scriptMapping.Trigger.TimeFrame.StartUtc;
        TimeSpan endUtc = scriptMapping.Trigger.TimeFrame.EndUtc;
        if (startUtc >= endUtc)
        {
            result.Errors.Add($"Script start time '{startUtc}' must be greater than end time '{endUtc}'.");
            return result;
        }
        
        TimeSpan currentTimeUtc = DateTime.UtcNow.TimeOfDay;

        if (currentTimeUtc <= startUtc || currentTimeUtc > endUtc)
        {
            result.Errors.Add($"The trigger has a constrain on the time of the day when it can be executed. " +
                "Please consult with the service owner about when this script can be launched.");
        }

        return result;
    }
  }
}