using System.Collections.Generic;

namespace Webhookshell.Models
{
    public class Trigger
    {
        public IList<HttpTriggerMethod?>? HttpMethods { get; set; }
        public IList<string> IpAddresses { get; set; }
        public IList<TimeFrame?> TimeFrames { get; set; }
    }
}