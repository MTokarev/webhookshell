namespace Webhookshell.Models
{
    /// <summary>
    /// Result object
    /// </summary>
    public class DtoResult
    {
        /// <summary>
        /// Script name
        /// </summary>
        public string ScriptName { get; set; }
        
        /// <summary>
        /// Optional message
        /// </summary>
        public string Message { get; set; }
        
        /// <summary>
        /// Script parameters
        /// </summary>
        public string Params { get; set; }
        
        /// <summary>
        /// Script output
        /// </summary>
        public string Output { get; set; }
    }
}