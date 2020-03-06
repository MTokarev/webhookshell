using System.Threading.Tasks;
using webhookshell.Models;

namespace webhookshell.Interfaces
{
    public interface IPswhService
    {
        public bool RunPswh(DTOPswh pswh);
    }
}