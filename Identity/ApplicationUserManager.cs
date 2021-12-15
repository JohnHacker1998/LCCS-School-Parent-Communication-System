using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;

namespace LCCS_School_Parent_Communication_System.Identity
{
    public class ApplicationUserManager:UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store): base(store)
        {

        }

        internal object DeleteAsync(Task<ApplicationUser> user)
        {
            throw new NotImplementedException();
        }
    }
}