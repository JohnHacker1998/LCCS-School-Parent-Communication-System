using LCCS_School_Parent_Communication_System.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace LCCS_School_Parent_Communication_System.viewModels
{
    public class RegistrarManagementViewModel
    {
        [Required(ErrorMessage = "Please enter full name")]
        [Display(Name = "Full Name")]
        [RegularExpression(@"^([a-z]\/|[A-Z]\/)?([a-z]|[A-Z]){2,}\s([a-z]\/|[A-Z]\/)?([a-z]|[A-Z]){2,}\s([a-z]\/|[A-Z]\/)?([a-z]|[A-Z]){2,}$", ErrorMessage = "Incorrect Full Name Format")]
        public string fullName { get; set; }
        [Required(ErrorMessage = "Please enter Email.")]
        [Display(Name = "Email")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Email is not valid.")]
        public string email { get; set; }

        public List<ApplicationUser> registrarList = new List<ApplicationUser>();
    }
}