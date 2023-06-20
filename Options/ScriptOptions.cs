using System.Collections.Generic;

namespace webhookshell.Options
{
    public class ScriptOptions
    {
        public string DefaultKey { get; set; }
        public IList<ScriptHandlerOptions> Handlers { get; set; }        
    }
}