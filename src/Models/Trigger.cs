using System.Collections.Generic;

namespace Webhookshell.Models
{
    public class Trigger
    {
        public HttpTriggerMethod? HttpMethod { get; set; }
        public List<string> IpAddresses { get; set; }
    }
}