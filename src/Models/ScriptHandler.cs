using System.Collections.Generic;

namespace Webhookshell.Models
{
    public class ScriptHandler
    {
        public string ScriptsLocation { get; set; }
        public string ProcessName { get; set; }
        public string FileExtension { get; set; }
        public string Key { get; set; }
        public List<ScriptMapping> ScriptsMapping { get; set; } = new();
    }
}