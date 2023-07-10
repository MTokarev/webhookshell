using System.ComponentModel.DataAnnotations;

namespace Webhookshell.Models
{
    public class DtoScript
    {
        [Required]
        public string Script { get; set; }
        public string Param { get; set; }
        [Required]
        public string Key { get; set; }
    }
}