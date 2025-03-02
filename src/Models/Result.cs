using System.Collections.Generic;

namespace Webhookshell.Models
{
    /// <summary>
    /// Generic result model
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Result<T>
    {
        /// <summary>
        /// True if the result doesn't have any errors
        /// </summary>
        public bool IsValid => Errors.Count == 0;
        
        /// <summary>
        /// False if the result has any errors
        /// </summary>
        public bool IsNotValid => Errors.Count > 0;

        /// <summary>
        /// Payload <seealso cref="T"/>
        /// </summary>
        public T Data { get; set; }
        
        /// <summary>
        /// Errors list
        /// </summary>
        public List<string> Errors = new();
    }
}