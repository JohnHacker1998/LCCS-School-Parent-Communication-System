using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using LCCS_School_Parent_Communication_System.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using LCCS_School_Parent_Communication_System.App_Start;

namespace LCCS_School_Parent_Communication_System.Identity
{
    public class ApplicationUser:IdentityUser
    {
        public string fullName { get; set; }

        internal Task<ClaimsIdentity> GenerateUserIdentityAsync(ApplicationUserManager userManager)
        {
            throw new NotImplementedException();
        }

        
    }
}