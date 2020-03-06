using System.ComponentModel.DataAnnotations;

namespace webhookshell.Models
{
    public class DTOPswh
    {
        [Required]
        public string script { get; set; }
        public string param { get; set; }
        [Required]
        public string key { get; set; }
    }
}