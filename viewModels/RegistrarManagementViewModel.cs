using LCCS_School_Parent_Communication_System.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LCCS_School_Parent_Communication_System.viewModels
{
    public class RegistrarManagementViewModel
    {
        public string fullName { get; set; }
        public string email { get; set; }

        public List<ApplicationUser> registrarList = new List<ApplicationUser>();
    }
}