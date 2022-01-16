using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LCCS_School_Parent_Communication_System.viewModels
{
    public class ProfileEditUserName
    {
        [Required(ErrorMessage = "UserName is Required")]
        [Display(Name = "UserName")]
        public string userName { get; set; }
    }
}