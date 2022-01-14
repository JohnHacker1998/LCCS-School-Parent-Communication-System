using LCCS_School_Parent_Communication_System.Identity;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LCCS_School_Parent_Communication_System.viewModels
{
    public class ProfileViewModel
    {
        public string fullName { get; set; }
        public string email { get; set; }
        public string userName { get; set; }

        public ApplicationUser user { get; set; }
    }
}