using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using LCCS_School_Parent_Communication_System.Models;

namespace LCCS_School_Parent_Communication_System.Identity
{
    public class ApplicationUser:IdentityUser
    {
        public string fullName { get; set; }

         }
}