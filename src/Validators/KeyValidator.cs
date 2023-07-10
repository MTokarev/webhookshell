using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Webhookshell.Interfaces;
using Webhookshell.Models;
using Webhookshell.Options;

namespace Webhookshell.Validators
{
  public class KeyValidator : IScriptValidator
  {
    private readonly ScriptOptions _options;

    public KeyValidator(IOptionsSnapshot<ScriptOptions> options)
    {
        _options = options.Value;
    }
    public Result<DtoResult> Validate(DtoScript scriptToCheck, ScriptHandler handler, HttpContext httpContext = null)
    {
        Result<DtoResult> result = new();

        var scriptMapping = handler.ScriptsMapping
            .Where(script => string.Equals(script.Name, scriptToCheck.Script, StringComparison.InvariantCultureIgnoreCase))
            .FirstOrDefault();

        // Assign the key using following priority (where 1 is the top priority):
        // 1. Script
        // 2. Handler
        // 3. Default
        string key = scriptMapping?.Key 
            ?? handler.Key 
            ?? _options.DefaultKey;

        if (!string.Equals(key, scriptToCheck.Key))
        {
            result.Errors.Add($"Invalid security key, please double check it and run command again.");
        }

        return result;
    }
  }
}