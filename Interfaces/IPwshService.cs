using System.Threading.Tasks;
using webhookshell.Models;

namespace webhookshell.Interfaces
{
    public interface IPwshService
    {
        public bool RunPswh(DTOPswh pswh);
    }
}