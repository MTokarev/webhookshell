using System.Collections.Generic;
using Webhookshell.Models;

namespace Webhookshell.Options
{
    public class ScriptOptions
    {
        public string DefaultKey { get; set; }
        public IList<ScriptHandler> Handlers { get; set; }
    }
}