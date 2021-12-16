using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace LCCS_School_Parent_Communication_System.viewModels
{
    public class LoginViewModel
    {

        [Required(ErrorMessage = "Please enter name"), MinLength(6)]
        [Display(Name = "Username")]
        public string username { get; set; }


        // public string email { get; set; }
        [Required(ErrorMessage = "Please enter Password"), MinLength(8)]
        [Display(Name = "Password")]
        public string password { get; set; }
       




    }
}