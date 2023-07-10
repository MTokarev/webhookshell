using System;
using System.Linq;
using Microsoft.Extensions.Options;
using Webhookshell.Interfaces;
using Webhookshell.Models;
using Webhookshell.Options;

namespace Webhookshell.Services
{
    public class HandlerDispatcher : IHandlerDispatcher
    {
        private readonly ScriptOptions _options;
        public HandlerDispatcher(IOptionsSnapshot<ScriptOptions> options)
        {
            _options = options.Value;
        }

        public Result<ScriptHandler> GetScriptHandler(DtoScript scriptToCheck)
        {
            Result<ScriptHandler> result = new();
            string scriptExtension = scriptToCheck
                .Script
                .Split(".", StringSplitOptions.RemoveEmptyEntries)
                .Last();

            if (scriptExtension is null)
            {
                result.Errors.Add($"Unable to extract script extension from '{scriptToCheck.Script}'.");
                return result;
            }

            ScriptHandler handler = _options
                .Handlers
                .Where(script => string.Equals(script.FileExtension, scriptExtension, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault();

            if (handler is null)
            {
                result.Errors.Add($"Unable to find a handler for the script extension '{scriptExtension}'. You need to add the handler to the service config.");

                return result;
            }

            result.Data = handler;

            return result;
        }
    }
}