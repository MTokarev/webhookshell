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

        if (scriptMapping.Trigger?.TimeFrames is null)
        {
            return result;
        }

        TimeSpan currentTimeUtc = DateTime.UtcNow.TimeOfDay;
        bool validFrameFound = false;
        
        foreach (TimeFrame timeFrame in scriptMapping.Trigger.TimeFrames)
        {
            if (timeFrame.StartUtc is null 
                || timeFrame.EndUtc is null)
            {
                result.Errors.Add("The calling script has a time constrain, but the 'Start' and 'End' time have not been set. "
                    + "Please consult with the service owner");
                return result;
            }

            if (timeFrame.StartUtc == timeFrame.EndUtc)
            {
                result.Errors.Add($"Script start time '{timeFrame.StartUtc}' must not be equal to the end time '{timeFrame.EndUtc}'.");
                return result;
            }
            // This is required to support cases such as:
            // Start 23:00:00 - End 07:00:00
            else if (timeFrame.StartUtc > timeFrame.EndUtc)
            {
                var oneDay = TimeSpan.FromDays(1);
                timeFrame.EndUtc = timeFrame.EndUtc.Value.Add(oneDay);
            }

            if (currentTimeUtc >= timeFrame.StartUtc && currentTimeUtc < timeFrame.EndUtc)
            {
                validFrameFound = true;
            }
        }

        if (!validFrameFound)
        {
            result.Errors.Add($"The trigger has a constrain on the time of the day when it can be executed. " +
                    "Please consult with the service owner about when this script can be launched.");
        }

        return result;
    }
  }
}