using Microsoft.AspNetCore.Http;
using Webhookshell.Models;

namespace Webhookshell.Interfaces
{
    public interface IScriptValidator
    {
        public Result<DtoResult> Validate(DtoScript scriptToCheck,
            ScriptHandler handler,
            HttpContext httpContext = default);
    }
}