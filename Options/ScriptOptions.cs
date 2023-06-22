using System.Collections.Generic;

namespace webhookshell.Options
{
    public class ScriptOptions
    {
        public string DefaultKey { get; set; }
        public Dictionary<string, string> ScriptsByKey { get; set; } = new();
        public IList<ScriptHandlerOptions> Handlers { get; set; }        
    }
}