namespace Webhookshell.Models
{
    public class ScriptMapping
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public Trigger? Trigger { get; set; }
    }
}