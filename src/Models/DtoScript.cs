using System.ComponentModel.DataAnnotations;

namespace Webhookshell.Models
{
    /// <summary>
    /// Script object
    /// </summary>
    public class DtoScript
    {
        /// <summary>
        /// Script name to execute
        /// </summary>
        [Required]
        public string Script { get; set; }
        
        /// <summary>
        /// Script parameters
        /// Example: 'param1=value1 param2=value2'
        /// </summary>
        public string Params { get; set; }
        
        /// <summary>
        /// Security key to run the script
        /// </summary>
        [Required]
        public string Key { get; set; }
    }
}