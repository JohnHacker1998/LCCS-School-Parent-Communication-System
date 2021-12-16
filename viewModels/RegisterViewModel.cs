using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace LCCS_School_Parent_Communication_System.viewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Please enter name"), MinLength(6)]
        [Display(Name = "Username")]
        public string username { get; set;}

        [Required(ErrorMessage = "Please enter Password"), MinLength(8)]
        [Display(Name = "Password")]
        public string password { get; set; }
        [Required(ErrorMessage = "Please enter full name")]
        [Display(Name = "Full Name")]
        [RegularExpression(@"([A-Z]{1}[a-z]{2,} ){2}([A-Z]{1}[a-z]{2,})",ErrorMessage ="Fullname isnot valid.")]
        public string fullName { get; set; }
        [Required(ErrorMessage = "Please enter Email.")]
        [Display(Name = "Email")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Email is not valid.")]

        public string email { get; set; }
    }
}