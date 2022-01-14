using Microsoft.Owin.Security.DataProtection;

namespace LCCS_School_Parent_Communication_System.Controllers
{
    internal class DataProtectorTokenProvider
    {
        private IDataProtector dataProtector;

        public DataProtectorTokenProvider(IDataProtector dataProtector)
        {
            this.dataProtector = dataProtector;
        }
    }
}