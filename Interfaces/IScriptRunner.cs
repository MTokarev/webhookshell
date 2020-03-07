using webhookshell.Models;

namespace webhookshell.Interfaces
{
    public interface IScriptRunner
    {
         public string Run(DTOScript scriptToRun);
    }
}