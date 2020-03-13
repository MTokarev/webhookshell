namespace webhookshell.Models
{
    public class DTOResult
    {
        public bool isCompletedSuccessfully { get; set; }
        public string scriptName { get; set; }
        public string message { get; set; }
        public string param { get; set; }
        public string output { get; set; }
    }
}