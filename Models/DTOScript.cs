using System.ComponentModel.DataAnnotations;

namespace webhookshell.Models
{
    public class DTOScript
    {
        [Required]
        public string Script { get; set; }
        public string Param { get; set; }
        [Required]
        public string Key { get; set; }
    }
}