using webhookshell.Models;

namespace webhookshell.Interfaces
{
    public interface IScriptRunner
    {
         public Result<DTOResult> Run(DTOScript scriptToRun);
    }
}