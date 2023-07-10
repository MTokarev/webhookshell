using Microsoft.AspNetCore.Http;
using Webhookshell.Models;

namespace Webhookshell.Interfaces
{
    public interface IScriptValidationService
    {
        public Result<DtoResult> Validate(DtoScript scriptToCheck, HttpContext httpContext = default);
    }
}