using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Webhookshell.Interfaces;
using Webhookshell.Models;

namespace Webhookshell.Services
{
    /// <summary>
    /// Script validation service
    /// Runs all registered script validators to check if the script is valid
    /// </summary>
    public class ScriptValidationService : IScriptValidationService
    {
        private readonly IHandlerDispatcher _handlerDispatcher;
        private readonly IEnumerable<IScriptValidator> _validators;

        public ScriptValidationService(IHandlerDispatcher handlerDispatcher,
            IEnumerable<IScriptValidator> validators)
        {
            _handlerDispatcher = handlerDispatcher;
            _validators = validators;
        }

        public Result<DtoResult> Validate(DtoScript scriptToCheck, HttpContext httpContext = null)
        {
            Result<DtoResult> result = new();

            Result<ScriptHandler> handlerResult = _handlerDispatcher.GetScriptHandler(scriptToCheck);
            if (handlerResult.IsNotValid)
            {
                result.Errors.AddRange(handlerResult.Errors);
                return result;
            }

            foreach (IScriptValidator validator in _validators)
            {
                Result<DtoResult> validatorResult = validator.Validate(scriptToCheck, handlerResult.Data, httpContext);

                if (validatorResult.IsNotValid)
                {
                    return validatorResult;
                }
            }

            return result;
        }
    }
}