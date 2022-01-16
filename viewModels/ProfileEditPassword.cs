using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LCCS_School_Parent_Communication_System.viewModels
{
    public class ProfileEditPassword
    {
        [Required(ErrorMessage = "Old Password is Required")]
        [Display(Name = "Old Password")]
        [DataType(DataType.Password)]
        public string oldPassword { get; set; }

        [Required(ErrorMessage = "New Password is Required")]
        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        public string newPassword { get; set; }

        [Required(ErrorMessage = "Confirm Password is Required")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("newPassword", ErrorMessage = "Password and Confirmation Password must match.")]
        public string confirmPassword { get; set; }
    }
}