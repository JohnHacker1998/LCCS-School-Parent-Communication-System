using LCCS_School_Parent_Communication_System.Identity;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LCCS_School_Parent_Communication_System.viewModels
{
    public class ProfileViewModel
    {
        
        [Display(Name = "NAME")]
        public string fullName { get; set; }

        [Display(Name = "EMAIL")]
        public string email { get; set; }

        [Display(Name = "USERNAME")]
        public string userName { get; set; }

        public ApplicationUser user { get; set; }
    }
}