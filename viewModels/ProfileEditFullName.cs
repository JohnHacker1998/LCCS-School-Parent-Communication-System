using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LCCS_School_Parent_Communication_System.viewModels
{
    public class ProfileEditFullName
    {
        [Required(ErrorMessage = "Full Name is Required")]
        [Display(Name = "Full Name")]
        [RegularExpression(@"^([a-z]\/|[A-Z]\/)?([a-z]|[A-Z]){2,}\s([a-z]\/|[A-Z]\/)?([a-z]|[A-Z]){2,}\s([a-z]\/|[A-Z]\/)?([a-z]|[A-Z]){2,}$", ErrorMessage = "Incorrect Full Name Format")]
        public string fullName { get; set; }

    }
}