using Microsoft.AspNetCore.Http;
using Webhookshell.Models;

namespace Webhookshell.Interfaces
{
    public interface IScriptRunnerService
    {
        public Result<DtoResult> Run(DtoScript scriptToRun, HttpContext httpContext);
    }
}