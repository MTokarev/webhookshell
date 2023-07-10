using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Webhookshell.Interfaces;
using Webhookshell.Models;
using Webhookshell.Options;

namespace Webhookshell.Validators
{
  public class IPAddressValidator : IScriptValidator
  {
    private readonly ScriptOptions _options;

    public IPAddressValidator(IOptionsSnapshot<ScriptOptions> options)
    {
        _options = options.Value;
    }

    public Result<DtoResult> Validate(DtoScript scriptToCheck, ScriptHandler handler, HttpContext httpContext = null)
    {
        Result<DtoResult> result = new();
        

        var scriptMapping = handler.ScriptsMapping
            .Where(script => string.Equals(script.Name, scriptToCheck.Script, StringComparison.InvariantCultureIgnoreCase))
            .FirstOrDefault();

        if (scriptMapping is null)
        {       
            return result;
        }

        if (scriptMapping.Trigger.IpAddresses is null || scriptMapping.Trigger.IpAddresses.Count() == 0)
        {
            return result;
        }
    
        if (httpContext == null)
        {
            result.Errors.Add("Http context must not be empty for IP Based validation");
        }

        string remoteAddress = httpContext.Connection
            .RemoteIpAddress
            .ToString();

        if (string.IsNullOrEmpty(remoteAddress))
        {
            result.Errors.Add("Unable to determine caller IP address for the script with address trigger.");

            return result;
        }

        if (!scriptMapping.Trigger.IpAddresses.Contains(remoteAddress))
        {
            result.Errors.Add($"Your IP address '{remoteAddress}' is not allowed to trigger the script execution.");
        }

        return result;
    }
  }
}