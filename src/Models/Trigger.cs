using System.Collections.Generic;

namespace Webhookshell.Models
{
    public class Trigger
    {
        public HttpTriggerMethod? HttpMethod { get; set; }
        public IList<string> IpAddresses { get; set; }
        public IList<TimeFrame?> TimeFrames { get; set; }
    }
}