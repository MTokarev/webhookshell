using System.Collections.Generic;
using webhookshell.Models;

namespace webhookshell.Options
{
    public class ScriptOptions
    {
        public string DefaultKey { get; set; }
        public Dictionary<string, ScriptByKey> ScriptsByKey { get; set; } = new();
        public IList<ScriptHandlerOptions> Handlers { get; set; }        
    }
}