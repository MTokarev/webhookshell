using System.Collections.Generic;

namespace Webhookshell.Models
{
    public class Result<T>
    {
        public bool IsValid => Errors.Count == 0;
        public bool IsNotValid => Errors.Count > 0;

        public T Data { get; set; }
        public List<string> Errors = new();
    }
}