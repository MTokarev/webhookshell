using Webhookshell.Models;

namespace Webhookshell.Interfaces
{
    public interface IHandlerDispatcher
    {
        public Result<ScriptHandler> GetScriptHandler(DtoScript scriptToCheck);
    }
}